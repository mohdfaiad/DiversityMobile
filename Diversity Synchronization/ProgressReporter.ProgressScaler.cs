﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UBT.AI4.Bio.DivMobi.DatabaseConnector.Serializer.Util;


namespace Diversity_Synchronization
{
    partial class ProgressReporter
    {
        private class ProgressScaler : IAdvanceProgress
        {
            bool progressIsSealed = true;
            int updateThreshold = 1;
            int internalTotal = 100;
            double externalTotal = 100d;
            int nextUpdate = 0;            
            ProgressReporter owner;

            public ProgressScaler(ProgressReporter owner)
            {
                this.owner = owner;   
                reset();
            }                   

            private void reset()
            {
                progressIsSealed = true;
                InternalCurrent = 0;
                InternalTotal = 1;
            }
            public void startProgress(double external, int internalTotal)
            {
                sealProgress();                
                progressIsSealed = false;
                InternalTotal = internalTotal;
                ExternalTotal = external;
                owner.IsProgressIndeterminate = false;
                owner.internalProgressChanged();
            }
           
            public void sealProgress()
            {
                if(!progressIsSealed)
                {
                    owner.advanceOverallProgress(ExternalTotal);
                    reset();
                    owner.internalProgressChanged();
                }                
            }

            public bool Running
            {
                get
                {
                    return !progressIsSealed;
                }
            }

            

            #region IInternalProgress Member

            public int InternalTotal
            {
                get
                {
                    return internalTotal;
                }
                set
                {
                    internalTotal = value;
                    calculateThreshold();
                }
            }

            public int InternalCurrent
            {
                get;
                set;
            }

            public double InternalCurrentPercentage
            {
                get
                {
                    return (((double)InternalCurrent / InternalTotal) * 100);
                }
            }

            public double ExternalTotal
            {
                get
                {
                    return externalTotal;
                }
                set
                {
                    externalTotal = value;
                    calculateThreshold();
                }
            }

            private void calculateThreshold()
            {
                int externalUpdateThreshold = (int)(0.01d * InternalTotal);
                int internalUpdateThreshold = (int)(1d / ExternalTotal) * InternalTotal;

                if(externalUpdateThreshold < internalUpdateThreshold)
                    updateThreshold = externalUpdateThreshold;
                else 
                    updateThreshold = internalUpdateThreshold;

                if (updateThreshold <= 0)
                    updateThreshold = 1;
            }

            public double ExternalCurrent
            {
                get
                {
                    return ((double)InternalCurrent / InternalTotal) * ExternalTotal;
                }
            }

            public void advance()
            {
                advance(1);
            }

            public void advance(int steps)
            {
                if (!progressIsSealed)
                {
                    if (InternalCurrent <= InternalTotal)
                    {
                        InternalCurrent += steps;

                        if (InternalCurrent > InternalTotal)
                            InternalCurrent = InternalTotal;

                        if (InternalCurrent >= nextUpdate)
                            owner.internalProgressChanged();
                        while (InternalCurrent >= nextUpdate)
                        {
                            nextUpdate += updateThreshold;
                        }
                    }
                    else
                        owner.internalProgressChanged();
                }
            }

            #endregion

            #region IReportProgress Member


            public bool IsCancelRequested
            {
                get { return owner.IsCancelRequested; }
            }

            #endregion
        }
    }
}