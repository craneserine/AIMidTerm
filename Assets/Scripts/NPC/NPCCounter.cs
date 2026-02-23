using UnityEngine;

public class NPCCounter : MonoBehaviour
{
    private int NPCCount = 0;
    
    public void AddNPC()
    {
        NPCCount++;
        Debug.Log($"{gameObject.name} now has {NPCCount} NPCs");
    }
    
    public void RemoveNPC()
    {
        NPCCount--;
        Debug.Log($"{gameObject.name} now has {NPCCount} NPCs");
    }
    
    public int GetNPCCount()
    {
        return NPCCount;
    }
}