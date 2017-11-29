using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.Drawing;

// TODO: If someone enter Json-executable code in a field that is Json serialized, will this present a security risk?

namespace LMS.ViewModels.Widgets
{
	/// <summary>
	/// ViewModel for populating a CollapsableList
	/// Using bootstrap-treeview package (https://github.com/jonmiles/bootstrap-treeview)
	/// </summary>
	public class TreeViewModel
	{
		[JsonIgnore]
		public string JsonObject => JsonConvert.SerializeObject(this, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

		public bool EnableLinks { get; set; }
		public bool ShowTags { get; set; }
		public bool ShowBorder { get; set; }
		public bool MultiSelect { get; set; }

		public Color BackColor { get; set; }
		public Color BorderColor { get; set; }
		public Color Color { get; set; }

		public string ExpandIcon { get; set; } = "glyphicon glyphicon-plus";
		public string CollapseIcon { get; set; } = "glyphicon glyphicon-minus";
		public string EmptyIcon { get; set; } = "glyphicon";
		public string CheckedIcon { get; set; } = "glyphicon glyphicon-check";

		public int Levels { get; set; }

		public ICollection<TreeViewNode> Data { get; set; }

		public class TreeViewNode
		{
			public string Text { get; set; }
			public string Icon { get; set; }
			public string SelectedIcon { get; set; }
			public Color Color { get; set; }
			public Color BackColor { get; set; }
			public string Href { get; set; }
			public bool Selectable { get; set; }
			public TreeViewNodeState State { get; set; }
			public string[] Tags { get; set; }
			public ICollection<TreeViewNode> Nodes { get; set; } = new List<TreeViewNode>();

			public struct TreeViewNodeState
			{
				public bool Checked { get; set; }
				public bool Disabled { get; set; }
				public bool Expanded { get; set; }
				public bool Selected { get; set; }
			}
		}

		/*
		public bool EnableLinks
		{
			get
			{
				Queue<TreeViewNode> nodesToTraverse = new Queue<TreeViewNode>(Data);

				while (nodesToTraverse.Count > 0)
				{
					TreeViewNode node = nodesToTraverse.Dequeue();

					if (node.Href != null)
						return true;

					foreach (var n in node.Nodes)
						nodesToTraverse.Enqueue(n);
				}

				return false;
			}
		}
		*/
	}
}
