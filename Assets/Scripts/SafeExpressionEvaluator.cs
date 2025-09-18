using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

public static class AEEvaluator
{
	private static Dictionary<string, List<(Instruction[], string[])>> instructionCache = new();
	private static string number = @"(?<NUM>[1-9][0-9]*|0)";
	private static string id = @"\$(?<ID>[A-Za-z]+)";
	private static string plus = @"(?<PLUS>\+)";
	private static string minus = @"(?<MINUS>-)";
	private static string mul = @"(?<MUL>\*)";
	private static string div = @"(?<DIV>/)";
	private static string lParen = @"(?<LPAREN>\()";
	private static string rParen = @"(?<RPAREN>\))";
	private static string blank = @"(?<BLANK>\s+)";

	private static Regex lexer = new Regex(@$"{number}|{id}|{plus}|{minus}|{mul}|{div}|{lParen}|{rParen}|{blank}",
		RegexOptions.Compiled);

	public static void RegisterExpression(string key, IEnumerable<string> expr)
	{
		instructionCache[key] = new();
		foreach (var e in expr)
		{
			instructionCache[key].Add(ShuntingYardCompiler.CompileToRpn(Tokenize(e)));
		}
	}

	public static List<int> GetExprValue(string key, IStat stat)
	{
		var ret = new List<int>();
		if (!instructionCache.TryGetValue(key, out var list)) return null;
		foreach (var (instr, vars) in list)
		{
			ret.Add(RpnEvaluator.Evaluate(instr, vars, stat));
		}

		return ret;
	}
	
	private enum TokenType
	{
		NUM,
		ID,
		PLUS,
		MINUS,
		MUL,
		DIV,
		LPAREN,
		RPAREN,
	}

	private struct Token
	{
		public TokenType Type;
		public string IdValue;

		public Token(TokenType type, string value = "")
		{
			Type = type;
			IdValue = value ?? "";
		}
	}

	private static List<Token> Tokenize(string input)
	{
		var tokens = new List<Token>();

		foreach (Match m in lexer.Matches(input))
		{
			if (m.Groups["BLANK"].Success) continue;

			if (m.Groups["NUM"].Success)
				tokens.Add(new Token(TokenType.NUM, m.Value));
			else if (m.Groups["ID"].Success)
				tokens.Add(new Token(TokenType.ID, m.Groups["ID"].Value));
			else if (m.Groups["PLUS"].Success)
				tokens.Add(new Token(TokenType.PLUS, m.Value));
			else if (m.Groups["MINUS"].Success)
				tokens.Add(new Token(TokenType.MINUS, m.Value));
			else if (m.Groups["MUL"].Success)
				tokens.Add(new Token(TokenType.MUL, m.Value));
			else if (m.Groups["DIV"].Success)
				tokens.Add(new Token(TokenType.DIV, m.Value));
			else if (m.Groups["LPAREN"].Success)
				tokens.Add(new Token(TokenType.LPAREN, m.Value));
			else if (m.Groups["RPAREN"].Success)
				tokens.Add(new Token(TokenType.RPAREN, m.Value));
		}

		return tokens;
	}

	enum OpCode : byte
	{
		PushConst,
		PushVar,
		Add,
		Sub,
		Mul,
		Div,
		Neg
	}

	readonly struct Instruction
	{
		public readonly OpCode Op;
		public readonly double Const;
		public readonly int VarIndex;

		public Instruction(OpCode op, double c = 0, int vi = -1)
		{
			Op = op;
			Const = c;
			VarIndex = vi;
		}
	}

	static class ShuntingYardCompiler
	{
		// 우선순위(단항 - 는 런타임에서 바로 방출하므로 스택에 오래 안 머무르게 처리)
		static int Prec(TokenType t) => t switch
		{
			TokenType.MUL or TokenType.DIV => 2,
			TokenType.PLUS or TokenType.MINUS => 1,
			_ => 0
		};

		public static (Instruction[] code, string[] varOrder) CompileToRpn(IReadOnlyList<Token> toks)
		{
			var outIns = new List<Instruction>(toks.Count * 2);
			var ops = new Stack<TokenType>();
			var varMap = new Dictionary<string, int>(StringComparer.Ordinal);
			var varOrder = new List<string>();

			int VarIndex(string name)
			{
				if (!varMap.TryGetValue(name, out var idx))
				{
					idx = varMap[name] = varOrder.Count;
					varOrder.Add(name);
				}

				return idx;
			}

			bool expectUnary = true;
			int pendingUnaryMinus = 0;


			for (int i = 0; i < toks.Count; i++)
			{
				var t = toks[i];
				switch (t.Type)
				{
					case TokenType.NUM:
					{
						if (!double.TryParse(t.IdValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var v))
							throw new FormatException($"Invalid number literal: '{t.IdValue}'");
						outIns.Add(new Instruction(OpCode.PushConst, v));
						expectUnary = false;
						break;
					}
					case TokenType.ID:
					{
						int vi = VarIndex(t.IdValue);
						outIns.Add(new Instruction(OpCode.PushVar, 0, vi));
						expectUnary = false;
						break;
					}
					case TokenType.LPAREN:
						ops.Push(TokenType.LPAREN);
						expectUnary = true;
						break;

					case TokenType.RPAREN:
						while (ops.Count > 0 && ops.Peek() != TokenType.LPAREN)
							EmitBinary(outIns, ops.Pop());
						if (ops.Count == 0 || ops.Peek() != TokenType.LPAREN)
							throw new Exception("Mismatched ')'");
						ops.Pop();
						expectUnary = false;
						break;

					case TokenType.PLUS:
					case TokenType.MINUS:
					case TokenType.MUL:
					case TokenType.DIV:
					{
						if (t.Type == TokenType.MINUS && expectUnary)
						{
							pendingUnaryMinus++;
							break;
						}

						while (ops.Count > 0 && ops.Peek() != TokenType.LPAREN &&
						       Prec(ops.Peek()) >= Prec(t.Type))
							EmitBinary(outIns, ops.Pop());

						ops.Push(t.Type);
						expectUnary = true;
						break;
					}
				}

				void ApplyPendingUnaryIfAny()
				{
					while (pendingUnaryMinus > 0)
					{
						outIns.Add(new Instruction(OpCode.Neg));
						pendingUnaryMinus--;
					}
				}

				switch (t.Type)
				{
					case TokenType.NUM:
					case TokenType.ID:
					case TokenType.RPAREN:
						ApplyPendingUnaryIfAny();
						break;
				}
			}

			while (ops.Count > 0)
			{
				var top = ops.Pop();
				if (top == TokenType.LPAREN) throw new Exception("Mismatched '('");
				EmitBinary(outIns, top);
			}

			if (pendingUnaryMinus != 0)
				throw new Exception("Dangling unary '-'");

			return (outIns.ToArray(), varOrder.ToArray());

			static void EmitBinary(List<Instruction> outIns, TokenType op)
			{
				outIns.Add(new Instruction(op switch
				{
					TokenType.PLUS => OpCode.Add,
					TokenType.MINUS => OpCode.Sub,
					TokenType.MUL => OpCode.Mul,
					TokenType.DIV => OpCode.Div,
					_ => throw new NotSupportedException($"Unsupported op {op}")
				}));
			}
		}
	}

	static class RpnEvaluator
	{
		public static int Evaluate(
			Instruction[] code,
			string[] varOrder,
			IStat stat)
		{
			var values = new double[varOrder.Length];
			for (int i = 0; i < varOrder.Length; i++)
			{
				if (!ValueType.TryParse(varOrder[i], out var valType))
					throw new KeyNotFoundException($"Variable '{varOrder[i]}' is not provided.");
				values[i] = stat.GetValueByValueType(valType);
			}

			return Evaluate(code, values);
		}

		public static int Evaluate(Instruction[] code, ReadOnlySpan<double> vars)
		{
			var stack = new double[code.Length];
			int sp = 0;

			foreach (ref readonly var ins in code.AsSpan())
			{
				switch (ins.Op)
				{
					case OpCode.PushConst:
						stack[sp++] = ins.Const;
						break;

					case OpCode.PushVar:
						stack[sp++] = vars[ins.VarIndex];
						break;

					case OpCode.Neg:
						stack[sp - 1] = -stack[sp - 1];
						break;

					case OpCode.Add:
					{
						double b = stack[--sp];
						double a = stack[--sp];
						stack[sp++] = a + b;
						break;
					}
					case OpCode.Sub:
					{
						double b = stack[--sp];
						double a = stack[--sp];
						stack[sp++] = a - b;
						break;
					}
					case OpCode.Mul:
					{
						double b = stack[--sp];
						double a = stack[--sp];
						stack[sp++] = a * b;
						break;
					}
					case OpCode.Div:
					{
						double b = stack[--sp];
						double a = stack[--sp];
						if (b == 0.0) throw new DivideByZeroException();
						stack[sp++] = a / b;
						break;
					}
					default:
						throw new NotSupportedException(ins.Op.ToString());
				}
			}

			if (sp != 1)
				throw new InvalidOperationException("Invalid RPN code: stack not singleton at end.");

			return (int)stack[0];
		}
	}
}