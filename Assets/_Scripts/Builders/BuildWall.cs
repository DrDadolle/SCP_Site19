﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildWall : MonoBehaviour, IBuildingMethod {

    // Tilemap containing the world
    public Tilemap map;

    // Instance for access purpose
    public static BuildWall Instance;

    // Store the ref to the mouse pointer
    public MousePointer pointer;

    // Starting Cell position of the drag and drop
    private Vector3Int startpos;
	int start_x;
	int start_y;

    // Current Cell position of the drag and drop
    private Vector3Int currentpos;

    // List of all pending tiles
    private List<Vector3Int> listOfPendingTiles = new List<Vector3Int>();

    //Storing potential Jobs
    private List<Job> listOfPotentialJobs = new List<Job>();

    // Storing old positions
    public class OldTileData
	{
		public Vector3Int npos;
		public TileBase tileBase;

		public OldTileData(Vector3Int npos, TileBase tileBase)
		{
			this.npos = npos;
			this.tileBase = tileBase;
		}
	}
    private List<OldTileData> listOfOldTiles = new List<OldTileData>();


    // On Awake
    void Awake()
    {
        Instance = this;
    }

    // ============================== Implement IBuildingMethod ==============================
    // These methods are called by the MouseController

    public void DuringDragAndDrop(TileBase tile)
    {
        UpdateTile(tile as WallTile);
    }

    public void OnLeftButtonPress(TileBase tile)
    {
		//Save starting position
		startpos = map.WorldToCell(pointer.GetWorldPoint());
		start_x = startpos.x;
		start_y = startpos.y;
    }

    public void OnLeftButtonReleaseDuringDragAndDrop(TileBase tile)
    {
        // Clean up the memory
        listOfOldTiles.Clear();

        // Add all jobs
        foreach(var j in listOfPotentialJobs)
        {
            JobManager.jobQueue.Enqueue(j);
        }
        // Change the preview tile to the pending tiles
        foreach (var v in listOfPendingTiles)
        {
            WallManager.Instance.GetWallModelFromDict(v).isPending = true;
            WallManager.Instance.GetWallModelFromDict(v).isPreview = false;
            map.RefreshTile(v);  
        }
        listOfPendingTiles.Clear();
        listOfPotentialJobs.Clear();
    }

    public void OnRightButtonPressDuringDragAndDrop(TileBase tile)
    {
        //Clear everything
        ClearAllLists();
    }

    public void OnKeyboardPress(TileBase tile)
    {
        //Do Nothing
    }

    public void OnUpdateWhenTileIsChanged(TileBase tile)
    {
        // TODO : Show Wall ?
    }

    // =========================== End Implement IBuildingMethod ==============================

    private void UpdateTile(WallTile tile)
    {

		// Avoid calling the updatetile method every frame if nothing changed
		if (currentpos == map.WorldToCell(pointer.GetWorldPoint()))
			return;


		// get current position
		currentpos = map.WorldToCell(pointer.GetWorldPoint());

		// Get starting and end position
		int end_x = currentpos.x;
		int end_y = currentpos.y;


		// Compare distance of X and Y, and set only on the biggest axis
		int end_max;
		int start_max;
		bool isBuildingOnXAxis;
		if (Mathf.Abs(end_x - start_x) >= Mathf.Abs(end_y - start_y)) {
			end_max = end_x;
			start_max = start_x;
			isBuildingOnXAxis = true;
		}
		else
		{
			end_max = end_y;
			start_max = start_y;
			isBuildingOnXAxis = false;
		}

		// We may be dragging in the "wrong" direction, so flip things if needed.
		if (end_max < start_max)
		{
			int tmp = end_max;
			end_max = start_max;
			start_max = tmp;
		}

        /**
         * Now start_x start_y is smaller to the end point
         */

        //Clear everything
        ClearAllLists();

        // Display a preview of the drag area
        for (int m = start_max; m <= end_max; m++)
		{
			Vector3Int npos;
			//Building on X
			if (isBuildingOnXAxis) {
				npos = new Vector3Int (m, start_y, 0);
			}
			// Building on Y
			else
			{
				npos = new Vector3Int (start_x, m, 0);
			}

            // If we are not building over an existing wall
            if (!(map.GetTile(npos) is WallTile))
            {
                //Add the old tile to the list
                OldTileData t_tmp = new OldTileData(npos, map.GetTile(npos));
                listOfOldTiles.Add(t_tmp);

                // Add model to WallmanagerDic
                WallModel _wall = new WallModel(npos, tile.wallData, true, false);
                // Null GO for now. Will be updated in WallTile StartUp method
                WallManager.Instance.listOfAllWalls.Add(_wall, null);

                map.SetTile(npos, tile);
                listOfPendingTiles.Add(npos);
                

                /**
                * JOBS !
                 * We should create all the jobs only on mousebutton release !
                * We should create a tmp list of jobs !
                */
                Job j_tmp = new Job(npos, (theJob) =>
                {
                    WallManager.Instance.GetWallModelFromDict(npos).isPending = false;
                    WallManager.Instance.GetWallModelFromDict(npos).isPreview = false;
                    map.RefreshTile(npos);

                }, tile.wallData.buildingTime);

                listOfPotentialJobs.Add(j_tmp);
            }
		}
    }

	/** Remove the previewTiles and add back the old one
     * FIXME special case with furnitures
     */
	private void CleanUpPreviewTiles()
	{
		while (listOfOldTiles.Count > 0)
		{
			OldTileData t = listOfOldTiles[0];
			listOfOldTiles.RemoveAt(0);
			map.SetTile(t.npos, t.tileBase);
		}
	}

    /**
     *  Clear every local lists
     */
     private void ClearAllLists()
    {
        // Clean up old drag previews
        CleanUpPreviewTiles();
        listOfPotentialJobs.Clear();

        //Remove all preview Walls Models before clearing tiles
        foreach (var v in listOfPendingTiles)
        {
            WallManager.Instance.RemoveWallModelFromDict(v);
        }
        listOfPendingTiles.Clear();

    }
}
