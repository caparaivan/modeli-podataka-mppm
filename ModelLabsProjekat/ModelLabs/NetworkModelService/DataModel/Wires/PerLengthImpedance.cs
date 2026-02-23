using System;
using System.Collections.Generic;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService.DataModel.Wires
{
    public class PerLengthImpedance : IdentifiedObject
    {
        private List<long> aCLineSegments = new List<long>();

        public PerLengthImpedance(long globalId) : base(globalId)
        {
        }

        public List<long> ACLineSegments
        {
            get { return aCLineSegments; }
            set { aCLineSegments = value; }
        }

        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
            {
                PerLengthImpedance x = (PerLengthImpedance)obj;
                return CompareHelper.CompareLists(x.aCLineSegments, aCLineSegments, true);
            }

            return false;
        }

        public override bool IsReferenced
        {
            get { return aCLineSegments.Count > 0 || base.IsReferenced; }
        }

        public override void AddReference(ModelCode referenceId, long globalId)
        {
            switch (referenceId)
            {
                case ModelCode.ACLINESEGMENT_PERLENGTHIMP:
                    aCLineSegments.Add(globalId);
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
                case ModelCode.ACLINESEGMENT_PERLENGTHIMP:
                    if (aCLineSegments.Contains(globalId))
                    {
                        aCLineSegments.Remove(globalId);
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