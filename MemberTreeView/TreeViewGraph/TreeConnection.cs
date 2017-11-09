using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeContainer
{
	public struct TreeConnection
	{
		public ITreeNode IgnParent { get; private set; }
		public ITreeNode IgnChild { get; private set; }
		public List<DPoint> LstPt { get; private set; }

		public TreeConnection(ITreeNode ignParent, ITreeNode ignChild, List<DPoint> lstPt) : this()
		{
			IgnChild = ignChild;
			IgnParent = ignParent;
			LstPt = lstPt;
		}
	}
}
