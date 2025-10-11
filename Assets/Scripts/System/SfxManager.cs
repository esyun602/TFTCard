
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SfxManager : MonoBehaviour
{
    public static SfxManager Instance { get; private set; }

    private Dictionary<string, AudioClip> clipCache = new();

    [Header("Pool")] public int initialPool = 16; // AudioSource 풀 크기
    public int maxPool = 64;

    [Header("Bus Volumes")] [Range(0f, 1f)]
    public float defaultVolume = 1f;

    [Range(0f, 1f)] public float uiVolume = 1f;
    [Range(0f, 1f)] public float combatVolume = 1f;
    [Range(0f, 1f)] public float envVolume = 1f;
    [Range(0f, 1f)] public float voiceVolume = 1f;

    readonly Queue<SfxSource> _free = new Queue<SfxSource>();
    readonly HashSet<SfxSource> _used = new HashSet<SfxSource>();
    readonly Dictionary<SfxInfo, float> _cooldowns = new Dictionary<SfxInfo, float>();
    readonly Dictionary<SfxInfo, int> _activeCount = new Dictionary<SfxInfo, int>();

    Transform _poolRoot;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _poolRoot = new GameObject("SFX_Pool").transform;
        _poolRoot.SetParent(transform);

        for (int i = 0; i < initialPool; i++)
            _free.Enqueue(CreateSource());
    }

    void Update()
    {
        if (_cooldowns.Count == 0) return;
        var keys = new List<SfxInfo>(_cooldowns.Keys);
        float dt = Time.unscaledDeltaTime;
        foreach (var k in keys)
        {
            _cooldowns[k] -= dt;
            if (_cooldowns[k] <= 0f) _cooldowns.Remove(k);
        }
    }

    SfxSource CreateSource()
    {
        var go = new GameObject("SFX_Audio");
        go.transform.SetParent(_poolRoot);
        var ss = go.AddComponent<SfxSource>();
        ss.onFinished = OnSourceFinished;
        return ss;
    }

    void OnSourceFinished(SfxSource s)
    {
        if (s.Info != null)
        {
            if (_activeCount.TryGetValue(s.Info, out int n))
                _activeCount[s.Info] = Mathf.Max(0, n - 1);
        }

        _used.Remove(s);
        _free.Enqueue(s);
        if (_free.Count > maxPool)
        {
            var x = _free.Dequeue();
            if (Application.isPlaying) Destroy(x.gameObject);
            else DestroyImmediate(x.gameObject);
        }
    }

    SfxSource Rent()
    {
        if (_free.Count == 0)
            _free.Enqueue(CreateSource());
        var s = _free.Dequeue();
        _used.Add(s);
        return s;
    }

    float GetBusVolume(SfxBus bus)
    {
        return bus switch
        {
            SfxBus.UI => uiVolume,
            SfxBus.Combat => combatVolume,
            SfxBus.Environment => envVolume,
            SfxBus.Voice => voiceVolume,
            _ => defaultVolume
        };
    }

    bool CanPlay(SfxInfo info)
    {
        if (info == null) return false;
        if (info.clip == null) return false;

        // 쿨다운
        if (_cooldowns.ContainsKey(info)) return false;

        // 인스턴스 제한
        if (info.maxInstances > 0 && _activeCount.TryGetValue(info, out int n) && n >= info.maxInstances)
            return false;

        return true;
    }

    void MarkPlayed(SfxInfo info)
    {
        if (info.cooldown > 0f)
            _cooldowns[info] = info.cooldown;

        if (!_activeCount.ContainsKey(info)) _activeCount[info] = 0;
        _activeCount[info]++;
    }

    // -------------------- Public API ----------------------
    public SfxSource Play2D(SfxInfo info)
    {
        if (!CanPlay(info)) return null;
        var s = Rent();
        s.src.volume = GetBusVolume(info.bus);
        s.followTarget = null;
        s.Play(info, Vector3.zero, false);
        s.src.spatialBlend = 0f; // 강제 2D
        MarkPlayed(info);
        return s;
    }

    public AudioClip GetClip(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return null;
        }
        
        if (!clipCache.TryGetValue(name, out var clip))
        {
            clipCache[name] = clip = Resources.Load<AudioClip>("Sfx/" + name);
        }

        return clip;
    }
    
    public SfxSource Play2D(string name)
    {
        var clip = GetClip(name);
        if (clip == null) return null;
        return Play2D(new SfxInfo(clip));
    }

    public SfxSource PlayAt(SfxInfo info, Vector3 worldPos)
    {
        if (!CanPlay(info)) return null;
        var s = Rent();
        s.src.volume = GetBusVolume(info.bus);
        s.followTarget = null;
        s.Play(info, worldPos, false);
        MarkPlayed(info);
        return s;
    }
    
    public SfxSource PlayAt(string name, Vector3 worldPos)
    {
        var clip = GetClip(name);
        if (clip == null) return null;
        return PlayAt(new SfxInfo(clip), worldPos);
    }
    
    public SfxSource PlayOn(SfxInfo info, Transform follow)
    {
        if (!CanPlay(info)) return null;
        var s = Rent();
        s.src.volume = GetBusVolume(info.bus);
        s.followTarget = follow;
        s.transform.position = follow.position;
        s.Play(info, follow.position, true);
        MarkPlayed(info);
        return s;
    }
    
    public SfxSource PlayOn(string name, Transform follow)
    {
        var clip = GetClip(name);
        if (clip == null) return null;
        return PlayOn(new SfxInfo(clip), follow);
    }

    public void StopAll()
    {
        foreach (var s in _used)
            s.StopImmediate();
    }

    // 런타임 전체 볼륨 스케일 (마스터)
    [Range(0f, 1f)] public float masterVolume = 1f;

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (Mathf.Approximately(masterVolume, 1f)) return;
        for (int i = 0; i < data.Length; i++) data[i] *= masterVolume;
    }
}