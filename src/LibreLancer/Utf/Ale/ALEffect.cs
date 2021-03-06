﻿/* The contents of this file are subject to the Mozilla Public License
 * Version 1.1 (the "License"); you may not use this file except in
 * compliance with the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 * 
 * Software distributed under the License is distributed on an "AS IS"
 * basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See the
 * License for the specific language governing rights and limitations
 * under the License.
 * 
 * 
 * The Initial Developer of the Original Code is Callum McGing (mailto:callum.mcging@gmail.com).
 * Portions created by the Initial Developer are Copyright (C) 2013-2016
 * the Initial Developer. All Rights Reserved.
 */
using System;
using System.Collections.Generic;
using System.Linq;
namespace LibreLancer.Utf.Ale
{
	public class ALEffect
	{
		public string Name;
		public uint CRC;
		public List<AlchemyNodeRef> FxTree;
		public List<AlchemyNodeRef> Fx;
		public List<Tuple<uint,uint>> Pairs;
		public ALEffect ()
		{
		}
		public AlchemyNodeRef FindRef(uint index)
		{
			var result = from AlchemyNodeRef r in Fx where r.Index == index select r;
			if (result.Count() == 1)
				return result.First();
			throw new Exception();
		}
	}
}

