public class Node
{
	public int x;
	public int y;
	public float g = 0;
	public float h = 0;
	public Node parent;

	public Node(int x, int y) {
		this.x = x;
		this.y = y;
	}

	public float f {
		get { return h + g; }
	}
}