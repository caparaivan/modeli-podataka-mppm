using System;
using System.Collections.Generic;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class ConductingEquipment : Equipment
    {
        private readonly List<long> terminals = new List<long>();

        public ConductingEquipment(long globalId) : base(globalId)
        {
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            if (referenceId == ModelCode.TERMINAL_CONDEQ)
            {
                if (!terminals.Contains(globalId)) terminals.Add(globalId);
            }
            else base.AddReference(referenceId, globalId);
        }

        public override void GetProperty(Property property)
        {
            if (property.Id == ModelCode.CONDEQ_TERMINALS)
            {
                property.SetValue(terminals);
            }
            else base.GetProperty(property);
        }

        public override bool IsReferenced
        {
            get { return terminals.Count > 0 || base.IsReferenced; }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            if (referenceId == ModelCode.TERMINAL_CONDEQ)
            {
                if (terminals.Contains(globalId))
                {
                    terminals.Remove(globalId);
                }
                else
                {
                    CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                }
            }
            else
            {
                base.RemoveReference(referenceId, globalId);
            }
        }
    }
}