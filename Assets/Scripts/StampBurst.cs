using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StampBurst {

	public class CubeArgs {
		public int row;
		public CubeController.Type type;
	};

	private static float DelayBetweenCubes = 0.6f;
	private StampManager manager;
	private int updateCount;
	class Burst {
		public int cubeDelay;
		public int cubeCount;
		public CubeController.Type cubeType;
		public List<int> columns;
	}
	private List<Burst> burstsInRow;
	private float deltaSum;

	private int[,] burstMap;
	private bool isBurstMap;
	private int burstMapCols, burstMapRows;

	public StampBurst(StampManager stampManager) {
		manager = stampManager;
		isBurstMap = false;

		burstsInRow = new List<Burst> ();
		for (int i=0; i < stampManager.getRowMax(); i++) {
			burstsInRow.Add(null);
		}

		updateCount = 0;
		deltaSum = 0;
	}

	public List<CubeArgs> getArgsForCubes(){
		deltaSum += Time.deltaTime;
		if (deltaSum < DelayBetweenCubes) {
			return new List<CubeArgs> ();
		}

		updateCount++;
		deltaSum = 0;

		if (isBurstMap) {
			return getArgsFromMap ();
		} else {
			return getArgsFromRows ();
		}
	}

	public List<CubeArgs> getArgsFromMap(){
		List<CubeArgs> args = new List<CubeArgs> ();
		int curCol = updateCount - 1;
		if (curCol >= burstMapCols) {
			return args;
		}

		for (int i=0; i < burstMapRows; i++) {
			if (burstMap[i, curCol] != (int) CubeController.Type.TYPE_NONE){
				CubeArgs c = new CubeArgs();
				c.row = i;
				c.type = (CubeController.Type) burstMap[i, curCol];
				args.Add(c);
			}
		}
		return args;
	}

	public List<CubeArgs> getArgsFromRows(){
		List<CubeArgs> args = new List<CubeArgs> ();
		for (int i=0; i < burstsInRow.Count; i++) {
			if ((burstsInRow[i] != null) && 
			    (updateCount % burstsInRow[i].cubeDelay == 0) &&
			    (burstsInRow[i].cubeCount > 0)){
				
				CubeArgs c = new CubeArgs();
				c.row = i;
				c.type = burstsInRow[i].cubeType;
				args.Add(c);

				burstsInRow[i].columns.RemoveAt(0);

				burstsInRow[i].cubeCount--;
				if (burstsInRow[i].cubeCount == 0){
					burstsInRow[i] = null;
				}
			}
		}
		return args;
	}

	public bool isRunning() {
		return (manager.getStampCount() > 0);
	}

	public int prepare(int burstMax, int cubeDelayMax, int cubeMax, bool powerTime) {
		int bcnt = Random.Range (1, burstMax + 1);
		for (int i = 0; i < bcnt; i++) {
			int row;
			do{
				row = Random.Range (0, burstsInRow.Count);
			}while(burstsInRow[row] != null);

			Burst b = new Burst();
			b.cubeType = CubeController.Type.TYPE_NONE;
			b.cubeCount = Random.Range(1, cubeMax + 1);
			b.cubeDelay = Random.Range (1, cubeDelayMax + 2);
			b.columns = new List<int>();
			for (int c=0; c < b.cubeCount; c++){
				b.columns.Add (manager.spawnStamp(row, powerTime));
			}
			b.columns.Sort();
			burstsInRow[row] = b;
		}
		isBurstMap = false;
		updateCount = 0;

		return bcnt;
	}

	public void prepareFromMap(int[,] stampMap, int rows, int cols, bool powerTime) {
		for (int i=0; i < rows; i++) {
			for (int j=0; j < cols; j++){
				if (stampMap[i,j] != (int) CubeController.Type.TYPE_NONE){
					manager.spawnStampAt (i, j, (CubeController.Type)stampMap[i,j], powerTime);
				}
			}
		}
		isBurstMap = true;
		burstMap = stampMap;
		burstMapRows = rows;
		burstMapCols = cols;
		updateCount = 0;
	}
}
