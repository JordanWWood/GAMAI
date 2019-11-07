using UnityEngine;

public class NavEdge {
    public Vector3 From { get; private set; }
    public Vector3 To { get; private set; }
    public float Cost { get; set; }
    public float Length { get; set; }
    
    public int Index;
    
    public NavEdge(Vector3 from, Vector3 to, float cost, float length, int index) {
        From = from;
        To = to;
        Cost = cost;
        Index = index;
        Length = length;
    }
}