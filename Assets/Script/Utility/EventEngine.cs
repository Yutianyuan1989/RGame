using System;
using System.Collections.Generic;

namespace Engine.Utility
{
    public class EventEngine
    {
        public delegate void EventCallback(int nEventID, object param);

        public delegate bool VoteCallback(int nEventID, object param);

        public delegate IVoteReason VoteCallBackReturnReason(int nEventID, object param);

        private static EventEngine _inst = null;

        private Dictionary<int, List<EventEngine.EventCallback>> m_EventList = new Dictionary<int, List<EventEngine.EventCallback>>();

        private Dictionary<int, List<EventEngine.VoteCallback>> m_VoteList = new Dictionary<int, List<EventEngine.VoteCallback>>();

        private Dictionary<int, List<EventEngine.VoteCallBackReturnReason>> m_dicVote = new Dictionary<int, List<EventEngine.VoteCallBackReturnReason>>();

        public static EventEngine Instance()
        {
            if (EventEngine._inst == null)
            {
                EventEngine._inst = new EventEngine();
            }
            return EventEngine._inst;
        }

        public void AddEventListener(int nEventID, EventEngine.EventCallback callback)
        {
            if (this.m_EventList == null)
            {
                this.m_EventList = new Dictionary<int, List<EventEngine.EventCallback>>();
            }
            List<EventEngine.EventCallback> list = null;
            if (!this.m_EventList.TryGetValue(nEventID, out list))
            {
                list = new List<EventEngine.EventCallback>();
                this.m_EventList.Add(nEventID, list);
            }
            if (!list.Contains(callback))
            {
                list.Add(callback);
            }
        }

        public void RemoveEventListener(int nEventID, EventEngine.EventCallback callback)
        {
            List<EventEngine.EventCallback> list = null;
            if (this.m_EventList.TryGetValue(nEventID, out list))
            {
                list.Remove(callback);
            }
        }

        public void RemoveAllEventListener(int nEventID)
        {
            List<EventEngine.EventCallback> list = null;
            if (this.m_EventList.TryGetValue(nEventID, out list))
            {
                list.Clear();
            }
        }

        public void DispatchEvent(int nEventID, object param = null)
        {
            List<EventEngine.EventCallback> list = null;
            if (this.m_EventList.TryGetValue(nEventID, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        try
                        {
                            list[i](nEventID, param);
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.Log("Event Exception:{0}" + ex.ToString());
                        }
                    }
                }
            }
        }

        public void AddVoteListener(int nEventID, EventEngine.VoteCallback callback)
        {
            if (this.m_VoteList == null)
            {
                this.m_VoteList = new Dictionary<int, List<EventEngine.VoteCallback>>();
            }
            List<EventEngine.VoteCallback> list = null;
            if (!this.m_VoteList.TryGetValue(nEventID, out list))
            {
                list = new List<EventEngine.VoteCallback>();
                this.m_VoteList.Add(nEventID, list);
            }
            if (!list.Contains(callback))
            {
                list.Add(callback);
            }
        }

        public void RemoveVoteListener(int nEventID, EventEngine.VoteCallback callback)
        {
            List<EventEngine.VoteCallback> list = null;
            if (this.m_VoteList.TryGetValue(nEventID, out list))
            {
                list.Remove(callback);
            }
        }

        public void RemoveAllVoteListener(int nEventID)
        {
            List<EventEngine.VoteCallback> list = null;
            if (this.m_VoteList.TryGetValue(nEventID, out list))
            {
                list.Clear();
            }
        }

        public bool DispatchVote(int nEventID, object param = null)
        {
            List<EventEngine.VoteCallback> list = null;
            bool result;
            if (this.m_VoteList.TryGetValue(nEventID, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        bool flag = false;
                        try
                        {
                            flag = list[i](nEventID, param);
                        }
                        catch (Exception ex)
                        {
                            UnityEngine.Debug.Log("Event Exception:{0}" + ex.ToString());
                        }
                        if (!flag)
                        {
                            result = false;
                            return result;
                        }
                    }
                }
            }
            result = true;
            return result;
        }

        public void AddVoteListenerReturnReason(int nEventID, EventEngine.VoteCallBackReturnReason callback)
        {
            if (this.m_dicVote == null)
            {
                this.m_dicVote = new Dictionary<int, List<EventEngine.VoteCallBackReturnReason>>();
            }
            List<EventEngine.VoteCallBackReturnReason> list = null;
            if (!this.m_dicVote.TryGetValue(nEventID, out list))
            {
                list = new List<EventEngine.VoteCallBackReturnReason>();
                this.m_dicVote.Add(nEventID, list);
            }
            if (!list.Contains(callback))
            {
                list.Add(callback);
            }
        }

        public void RemoveVoteListenerReturnReason(int nEventID, EventEngine.VoteCallBackReturnReason callback)
        {
            List<EventEngine.VoteCallBackReturnReason> list = null;
            if (this.m_dicVote.TryGetValue(nEventID, out list))
            {
                list.Remove(callback);
            }
        }

        public void RemoveAllVoteListenerRetrurnReason(int nEventID)
        {
            List<EventEngine.VoteCallBackReturnReason> list = null;
            if (this.m_dicVote.TryGetValue(nEventID, out list))
            {
                list.Clear();
            }
        }

        public IVoteReason DispatchVoteReturnReason(int nEventID, object param = null)
        {
            List<EventEngine.VoteCallBackReturnReason> list = null;
            IVoteReason voteReason = null;
            IVoteReason result;
            if (this.m_dicVote.TryGetValue(nEventID, out list))
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] != null)
                    {
                        voteReason = list[i](nEventID, param);
                        if (voteReason == null)
                        {
                            result = null;
                            return result;
                        }
                        if (voteReason.ErrorID != 0)
                        {
                            result = voteReason;
                            return result;
                        }
                    }
                }
            }
            result = voteReason;
            return result;
        }
    }
}
