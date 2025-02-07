using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MessageSystem
{
    public interface IMessageReceiver
    {
        public void CatchMessage(Message m);
    }

}
