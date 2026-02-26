using System;
using System.Collections.Generic;
using FTN.Common;

namespace FTN.Services.NetworkModelService.DataModel.Core
{
    public class ConnectivityNode : IdentifiedObject
    {
        private List<long> terminals = new List<long>();

        public ConnectivityNode(long globalId) : base(globalId)
        {
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            if (referenceId == ModelCode.TERMINAL_CONNECTIVITYNODE)
            {
                if (!terminals.Contains(globalId)) terminals.Add(globalId);
            }
            else base.AddReference(referenceId, globalId);
        }

        public override void GetProperty(Property property)
        {
            if (property.Id == ModelCode.CONNECTIVITYNODE_TERMINALS)
            {
                property.SetValue(terminals);
            }
            else base.GetProperty(property);
        }

        public override bool HasProperty(ModelCode property)
        {
            if (property == ModelCode.CONNECTIVITYNODE_TERMINALS) return true;
            return base.HasProperty(property);
        }

        public List<long> Terminals
        {
            get { return terminals; }
            set { terminals = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                ConnectivityNode x = (ConnectivityNode)obj;
                return CompareHelper.CompareLists(x.terminals, terminals, true);
            }

            return false;
        }

        public override bool IsReferenced
        {
            get { return terminals.Count > 0 || base.IsReferenced; }
        }

        public override void RemoveReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONNECTIVITYNODE:
                    if (terminals.Contains(globalId))
                    {
                        terminals.Remove(globalId);
                    }
                    else
                    {
                        CommonTrace.WriteTrace(CommonTrace.TraceWarning, "Entity (GID = 0x{0:x16}) doesn't contain reference 0x{1:x16}.", this.GlobalId, globalId);
                    }
                    break;
                default:
                    base.RemoveReference(referenceId, globalId);
                    break;
            }
        }
    }
}