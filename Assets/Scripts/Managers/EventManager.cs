using System;
using System.Collections.Generic;

namespace Managers
{
    public enum GameEvent
    {
    }

    public class EventManager
    {
        private static Dictionary<GameEvent, Action<object[]>> _eventTable =
            new();

        public static void AddHandler(GameEvent gameEvent, Action<object[]> handler)
        {
            if (_eventTable.ContainsKey(gameEvent))
                _eventTable[gameEvent] += handler;
            else
                _eventTable[gameEvent] = handler;
        }

        public static void RemoveHandler(GameEvent gameEvent, Action<object[]> handler)
        {
            if (_eventTable.ContainsKey(gameEvent))
                _eventTable[gameEvent] -= handler;
        }

        public static void Broadcast(GameEvent gameEvent, params object[] args)
        {
            if(_eventTable.ContainsKey(gameEvent))
                _eventTable[gameEvent]?.Invoke(args);
        }
    }
}