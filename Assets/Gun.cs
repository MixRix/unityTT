using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Gun", menuName ="Gun")]
public class Gun : ScriptableObject
{
    public Mesh gunMesh;
    public int damage;
    public int magazineSize;
    public float reloadTime;
    public float TimeBetweenShooting;
    public float TimeBetweenShots;
    public bool AllowToHold;
    



}
