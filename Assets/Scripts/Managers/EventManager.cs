using System;
using System.Collections.Generic;

namespace Managers
{
    public enum GameEvent
    {
        OnHitTarget,
        OnJoinedRoom,
        OnPropertiesAssigned,
        OnGameOver
    }

    public static class EventManager
    {
        private static Dictionary<GameEvent, Delegate> eventTable =
            new Dictionary<GameEvent, Delegate>();

        public static void AddHandler(GameEvent gameEvent, Delegate @delegate)
        {
            if (eventTable.ContainsKey(gameEvent))
                eventTable[gameEvent] = Delegate.Combine(eventTable[gameEvent], @delegate);

            else
                eventTable[gameEvent] = @delegate;
        }

        public static void RemoveHandler(GameEvent gameEvent, Delegate @delegate)
        {
            if (eventTable.ContainsKey(gameEvent))
            {
                eventTable[gameEvent] = Delegate.Remove(eventTable[gameEvent], @delegate);
                if (eventTable[gameEvent] == null)
                    eventTable.Remove(gameEvent);
            }
        }

        public static void Broadcast(GameEvent gameEvent, params object[] args)
        {
            if (eventTable.ContainsKey(gameEvent))
            {
                Delegate @delegate = eventTable[gameEvent];
                @delegate.DynamicInvoke(args);
            }
        }
    }
}
