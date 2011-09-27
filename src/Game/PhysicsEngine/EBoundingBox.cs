using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public enum EBoundingBoxType
    {
        Body,
        Event
    }

    public enum EventBoundingBoxType
    {
        Internal,
        External
    }

    public class EBoundingBox : BoundingBox
    {
        static UInt32 EventCount = 0;
        String GetId() { return "EBB_" + EventCount++.ToString(); }
        // script variable name
        public String Id { get; private set; }

        public EBoundingBoxType Type { get; private set; }

        public List<ObjectEvent> Events { get; private set; }

        public EBoundingBox(WorldObject holder, EBoundingBoxType type, int left, int top, int right, int bottom) :
            base(holder, left, top, right, bottom)
        {
            Type = type;

            Events = new List<ObjectEvent>();
        }

        public EBoundingBox(EBoundingBox copy, WorldObject holder) :
            base(copy, holder)
        {
            Type = copy.Type;

            Events = new List<ObjectEvent>(copy.Events);
        }

        public void CallEvent(ObjectEventType type, EBoundingBox source, EBoundingBox trigger)
        {
            foreach (ObjectEvent evt in Events)
            {
                if (evt.Type != type)
                    continue;

                evt.CallEvent(source, trigger);
            }
        }

        public void CallEvent(ObjectEventType type, ObjectEventArgs args)
        {
            foreach (ObjectEvent evt in Events)
            {
                if (evt.Type != type)
                    continue;

                evt.CallEvent(args);
            }
        }

        public void AddEvent(ObjectEvent evt)
        {
            Events.Add(evt);
        }

        public String ToScript()
        {
            Id = GetId();

            String toScript = "";

            toScript += Id.ToString() + " = EBoundingBox ( " +
                Holder.Id +
                ", EBoundingBoxType.Event, " +
                BaseLeft.ToString() + ", " +
                BaseTop.ToString() + ", " +
                BaseRight.ToString() + ", " +
                BaseBottom.ToString() + " )\n";

            foreach (ObjectEvent objectEvent in Events)
            {
                objectEvent.SetId(Id);

                toScript += objectEvent.ToScript() + "\n";

                toScript += Id.ToString() + ":AddEvent ( " + objectEvent.Id + " )\n";
            }

            toScript += Holder.Id.ToString() + ":AddEventBoundingBox( " + Id + ", EventBoundingBoxType.External )";

            return toScript;
        }
    }
}
