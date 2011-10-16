using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

public class VoxSprite : MonoBehaviour {
	
	private Vector3 dims = new Vector3 (1, 1, 1);
	private char[][][] rawVox;
	private string description = "";
	private bool showAll = false;
	
	public TextAsset voxData;
	public Transform voxPrototype;

	// Use this for initialization
	void Start ()
	{		
		string strVox = "";
		
		if (voxData == null)
			throw new UnityException ("Voxel data required");
		
		if (voxPrototype == null)
			throw new UnityException ("Prototype Transform required");
				
		// read in voxel data, render in place of sprite..
		foreach (string line in voxData.text.Split ('\n')) {
			
				if (line.Contains (":")) {
				
				string[] lineComponents = line.Split (':');
				
				switch (lineComponents[0]) {
				
				case "Size":
					lineComponents = lineComponents[1].Split ('x');
					dims.x = System.Convert.ToSingle (lineComponents[0]);
					dims.y = System.Convert.ToSingle (lineComponents[1]);
					dims.z = System.Convert.ToSingle (lineComponents[2]);
					break;
				
				case "Description":
					description = lineComponents[1];
					break;
				
				case "Show All":
					showAll = lineComponents[1].Contains ("yes");
					break;
				}
			
			} else {
				
				strVox += line;
			}
		}
		
		strVox = Regex.Replace (strVox, "[^0-9]", "");
		char[] voxCharArray = new char[strVox.Length];
		StringReader sr = new StringReader (strVox);
		sr.Read (voxCharArray, 0, strVox.Length);
				
		rawVox = new char[(int)dims.x][][];
		
		int rawPos = 0;
		
		Quaternion originalRotation = transform.rotation;
		transform.rotation = new Quaternion (0, 0, 0, 0);
		
		for (int vx = 0; vx < dims.x; vx++) {
			
			rawVox[vx] = new char[(int)dims.y][];
		
			for (int vy = 0; vy < dims.y; vy++) {
				
				rawVox[vx][vy] = new char[(int)dims.z];
			
				for (int vz = 0; vz < dims.z; vz++) {
					
					if (voxCharArray[rawPos] == '0') {
						
						// transparency
						rawVox[vx][vy][vz] = '0';
						
					} else {
						
						// determine whether the vox is actually visible..
						// Address = ((depthindex*col_size+colindex) * row_size + rowindex)
						if (!showAll && vx > 0 && vy > 0 && vz > 0 && vx < dims.x - 1 && vy < dims.y - 1 && vz < dims.z - 1 && 
							voxCharArray[(int)((((vx + 1) * dims.z + vy) * dims.y + vz))] != '0' &&
								voxCharArray[(int)((((vx - 1) * dims.z + vy) * dims.y + vz))] != '0' &&
								voxCharArray[(int)(((vx * dims.z + (vy + 1)) * dims.y + vz))] != '0' &&
								voxCharArray[(int)(((vx * dims.z + (vy - 1)) * dims.y + vz))] != '0' &&
								voxCharArray[(int)(((vx * dims.z + vy) * dims.y + (vz + 1)))] != '0' &&
								voxCharArray[(int)(((vx * dims.z + vy) * dims.y + (vz - 1)))] != '0'
								 ) {
								 	
								 	// ignore..
						} else {
						
						// create game object and colour appropriately..
							GameObject go = (GameObject)Instantiate (voxPrototype.gameObject);
							go.transform.parent = this.transform;
							go.transform.localPosition = new Vector3 (vx * voxPrototype.localScale.x, vy * voxPrototype.localScale.y, vz * voxPrototype.localScale.z);
						
						if (go.renderer != null) {
								switch (voxCharArray[rawPos]) {
							
						case '1':
									go.renderer.material.color = Color.blue;
									break;
								case '2':
									go.renderer.material.color = Color.red;
									break;
								case '3':
									go.renderer.material.color = Color.green;
									break;
								case '4':
									go.renderer.material.color = Color.yellow;
									break;
								case '5':
									go.renderer.material.color = Color.cyan;
									break;
								case '6':
									go.renderer.material.color = Color.magenta;
									break;
								case '7':
									go.renderer.material.color = Color.grey;
									break;
								case '8':
									go.renderer.material.color = Color.black;
									break;
								case '9':
									go.renderer.material.color = Color.white;
									break;
								}
							}
						}
					}
					
					rawPos++;
				}
			}
		}
		
		transform.rotation = originalRotation;
	}	
}
