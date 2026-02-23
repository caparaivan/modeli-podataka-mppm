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

        public override bool HasProperty(ModelCode property)
        {
            switch (property)
            {
                case ModelCode.CONNECTIVITYNODE_TERMINALS:
                    return true;
                default:
                    return base.HasProperty(property);
            }
        }

        public override void GetProperty(Property property)
        {
            switch (property.Id)
            {
                case ModelCode.CONNECTIVITYNODE_TERMINALS:
                    property.SetValue(terminals);
                    break;
                default:
                    base.GetProperty(property);
                    break;
            }
        }

        public override bool IsReferenced
        {
            get { return terminals.Count > 0 || base.IsReferenced; }
        }

        public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
        {
            if (terminals != null && terminals.Count > 0 && (refType == TypeOfReference.Target || refType == TypeOfReference.Both))
            {
                references[ModelCode.CONNECTIVITYNODE_TERMINALS] = terminals.GetRange(0, terminals.Count);
            }

            base.GetReferences(references, refType);
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.TERMINAL_CONNECTIVITYNODE:
                    terminals.Add(globalId);
                    break;
                default:
                    base.AddReference(referenceId, globalId);
                    break;
            }
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