namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
	using FTN.Common;

	/// <summary>
	/// PowerTransformerConverter has methods for populating
	/// ResourceDescription objects using PowerTransformerCIMProfile_Labs objects.
	/// </summary>
	public static class PowerTransformerConverter
	{

		#region Populate ResourceDescription
		public static void PopulateIdentifiedObjectProperties(FTN.IdentifiedObject cimIdentifiedObject, ResourceDescription rd)
		{
			if ((cimIdentifiedObject != null) && (rd != null))
			{
				if (cimIdentifiedObject.MRIDHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimIdentifiedObject.MRID));
				}
				if (cimIdentifiedObject.NameHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimIdentifiedObject.Name));
				}
				if (cimIdentifiedObject.DescriptionHasValue)
				{
					rd.AddProperty(new Property(ModelCode.IDOBJ_DESCRIPTION, cimIdentifiedObject.Description));
				}
			}
		}

        public static void PopulateConnectivityNodeProperties(FTN.ConnectivityNode cimConnectivityNode, ResourceDescription rd)
        {
            if ((cimConnectivityNode != null) && (rd != null))
            {
				PopulateIdentifiedObjectProperties(cimConnectivityNode, rd);
			}
		}

        public static void PopulateTerminalProperties(FTN.Terminal cimTerminal, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimTerminal != null) && (rd != null))
            {
                PopulateIdentifiedObjectProperties(cimTerminal, rd);
                
				if (cimTerminal.ConductingEquipmentHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimTerminal.ConductingEquipment.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimTerminal.GetType().ToString()).Append(" rdfID = \"").Append(cimTerminal.ID);
                        report.Report.Append("\" - Failed to set reference to ConductingEquipment: rdfID \"").Append(cimTerminal.ConductingEquipment.ID).AppendLine("\" is not mapped to GID!");
                    }

                    rd.AddProperty(new Property(ModelCode.TERMINAL_CONDEQ, gid));
                }
                if (cimTerminal.ConnectivityNodeHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimTerminal.ConnectivityNode.ID); if (gid < 0)
					{
                        report.Report.Append("WARNING: Convert ").Append(cimTerminal.GetType().ToString()).Append(" rdfID = \"").Append(cimTerminal.ID);
                        report.Report.Append("\" - Failed to set reference to ConnectivityNode: rdfID \"").Append(cimTerminal.ConnectivityNode.ID).AppendLine("\" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.TERMINAL_CONNECTIVITYNODE, gid));
                }
			}
		}

        public static void PopulateConductingEquipmentProperties(FTN.ConductingEquipment cimConductingEquipment, ResourceDescription rd)
        {
            if ((cimConductingEquipment != null) && (rd != null))
            {
                PopulateIdentifiedObjectProperties(cimConductingEquipment, rd);
            }
		}

        public static void PopulateConductorProperties(FTN.Conductor cimConductor, ResourceDescription rd)
        {
            if ((cimConductor != null) && (rd != null))
            {
                PopulateConductingEquipmentProperties(cimConductor, rd);
                if (cimConductor.LengthHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.CONDUCTOR_LENGTH, cimConductor.Length));
                }
			}
		}

        public static void PopulateACLineSegmentProperties(FTN.ACLineSegment cimACLineSegment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimACLineSegment != null) && (rd != null))
            {
                PopulateConductorProperties(cimACLineSegment, rd);

                if (cimACLineSegment.RHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_R, cimACLineSegment.R));
                }
                if (cimACLineSegment.R0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_R0, cimACLineSegment.R0));
                }
                if (cimACLineSegment.XHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_X, cimACLineSegment.X));
                }
                if (cimACLineSegment.X0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_X0, cimACLineSegment.X0));
                }
                if (cimACLineSegment.BchHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_BCH, cimACLineSegment.Bch));
                }
                if (cimACLineSegment.B0chHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_B0CH, cimACLineSegment.B0ch));
                }
                if (cimACLineSegment.GchHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_GCH, cimACLineSegment.Gch));
                }
                if (cimACLineSegment.G0chHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_G0CH, cimACLineSegment.G0ch));
                }
                if (cimACLineSegment.PerLengthImpedanceHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimACLineSegment.PerLengthImpedance.ID);
					
					if (gid < 0)
					{
                        report.Report.Append("WARNING: Convert ").Append(cimACLineSegment.GetType().ToString()).Append(" rdfID = \"").Append(cimACLineSegment.ID);
                        report.Report.Append("\" - Failed to set reference to PerLengthImpedance: rdfID \"").Append(cimACLineSegment.PerLengthImpedance.ID).AppendLine("\" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.ACLINESEGMENT_PERLENGTHIMP, gid));
                }
			}
		}

        public static void PopulateDCLineSegmentProperties(FTN.DCLineSegment cimDCLineSegment, ResourceDescription rd)
        {
            if ((cimDCLineSegment != null) && (rd != null))
            {
                PopulateConductorProperties(cimDCLineSegment, rd);
            }
		}

        public static void PopulateSeriesCompensatorProperties(FTN.SeriesCompensator cimSeriesCompensator, ResourceDescription rd)
        {
            if ((cimSeriesCompensator != null) && (rd != null))
            {
                PopulateConductingEquipmentProperties(cimSeriesCompensator, rd);

                if (cimSeriesCompensator.RHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SERIESCOMPENSATOR_R, cimSeriesCompensator.R));
                }
                if (cimSeriesCompensator.R0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SERIESCOMPENSATOR_R0, cimSeriesCompensator.R0));
                }
                if (cimSeriesCompensator.XHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SERIESCOMPENSATOR_X, cimSeriesCompensator.X));
                }
                if (cimSeriesCompensator.X0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SERIESCOMPENSATOR_X0, cimSeriesCompensator.X0));
                }
            }
        }

        public static void PopulatePerLengthSequenceImpedanceProperties(FTN.PerLengthSequenceImpedance cimPerLengthSequenceImpedance, ResourceDescription rd)
        {
            if ((cimPerLengthSequenceImpedance != null) && (rd != null))
            {
                PopulateIdentifiedObjectProperties(cimPerLengthSequenceImpedance, rd);
                if (cimPerLengthSequenceImpedance.RHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPEDANCE_R, cimPerLengthSequenceImpedance.R));
                }
                if (cimPerLengthSequenceImpedance.R0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPEDANCE_R0, cimPerLengthSequenceImpedance.R0));
                }
                if (cimPerLengthSequenceImpedance.XHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPEDANCE_X, cimPerLengthSequenceImpedance.X));
                }
                if (cimPerLengthSequenceImpedance.X0HasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPEDANCE_X0, cimPerLengthSequenceImpedance.X0));
                }
                if (cimPerLengthSequenceImpedance.BchHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPEDANCE_BCH, cimPerLengthSequenceImpedance.Bch));
                }
                if (cimPerLengthSequenceImpedance.B0chHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPEDANCE_B0CH, cimPerLengthSequenceImpedance.B0ch));
                }
                if (cimPerLengthSequenceImpedance.GchHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPEDANCE_GCH, cimPerLengthSequenceImpedance.Gch));
                }
                if (cimPerLengthSequenceImpedance.G0chHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.PERLENGTHSEQUENCEIMPEDANCE_G0CH, cimPerLengthSequenceImpedance.G0ch));
                }
            }
		}
		#endregion Populate ResourceDescription
	}
}
