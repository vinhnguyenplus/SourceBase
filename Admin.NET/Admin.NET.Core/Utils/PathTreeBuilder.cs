// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

/// <summary>
/// tree node
/// </summary>
public class TreeNode
{
    public int Id { get; set; }
    public int Pid { get; set; }
    public string Name { get; set; }
    public List<TreeNode> Children { get; set; } = new();
}

/// <summary>
/// Generate tree structure based on path array
/// </summary>
public class PathTreeBuilder
{
    private int _nextId = 1;

    public TreeNode BuildTree(List<string> paths)
    {
        var root = new TreeNode { Id = 1, Pid = 0, Name = "File directory" }; // root node
        var dict = new Dictionary<string, TreeNode>();

        foreach (var path in paths)
        {
            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            TreeNode currentNode = root;

            foreach (var part in parts)
            {
                var key = currentNode.Id + "_" + part; // Generate unique key
                if (!dict.ContainsKey(key))
                {
                    var newNode = new TreeNode
                    {
                        Id = _nextId++,
                        Pid = currentNode.Id,
                        Name = part
                    };
                    currentNode.Children.Add(newNode);
                    dict[key] = newNode;
                }
                currentNode = dict[key]; // Update current node
            }
        }

        return root;
    }
}