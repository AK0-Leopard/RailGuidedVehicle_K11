﻿// ***********************************************************************
// Assembly         : ScriptControl
// Author           : 
// Created          : 03-31-2016
//
// Last Modified By : 
// Last Modified On : 03-24-2016
// ***********************************************************************
// <copyright file="S10F3.cs" company="">
//     Copyright ©  2014
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mirle.ibg3k0.stc.Common;
using com.mirle.ibg3k0.stc.Data.SecsData;

namespace com.mirle.ibg3k0.sc.Data.SECS.AGVC.ASE
{
    /// <summary>
    /// Class S10F3.
    /// </summary>
    /// <seealso cref="com.mirle.ibg3k0.stc.Data.SecsData.SXFY" />
    public class S10F3 : SXFY
    {
        /// <summary>
        /// Terminal number
        /// </summary>
        [SecsElement(Index = 1, Type = SecsElement.SecsElementType.TYPE_BINARY, Length = 1)]
        public string TID;
        /// <summary>
        /// The text
        /// </summary>
        [SecsElement(Index = 2, Type = SecsElement.SecsElementType.TYPE_ASCII, Length = 120)]
        public string TEXT;

        /// <summary>
        /// Initializes a new instance of the <see cref="S10F3"/> class.
        /// </summary>
        public S10F3() 
        {
            StreamFunction = "S10F3";
            W_Bit = 1;
        }
    }
}
