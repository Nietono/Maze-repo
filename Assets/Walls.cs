﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour {

    public GameObject wallN;
    public GameObject wallE;
    public GameObject wallS;
    public GameObject wallW;

    private int myX = 0;
    private int myY = 0;
    private int maxX = 0;
    private int maxY = 0;

    public Transform neighbourN;
    public Transform neighbourE;
    public Transform neighbourS;
    public Transform neighbourW;

    public void Initialise(int x, int y, int newMaxX, int newMaxY)
    {
        myX = x;
        myY = y;
        maxX = newMaxX;
        maxY = newMaxY;

        FixUVs();
    }

    public void FixWallUVs(Transform wallTransform)
    {
        Mesh currentMesh = wallTransform.GetComponent<MeshFilter>().mesh;
        float currentWidth = wallTransform.localScale.x * 0.1f;
        float currentHeight = wallTransform.localScale.y * 0.1f;
        float currentDepth = wallTransform.localScale.z * 0.1f;
        Vector2[] newUVs = new Vector2[24];

        for (int i = 0; i < currentMesh.uv.Length; i++)
        {
            //Front
            newUVs[2] = new Vector2(0, currentHeight);
            newUVs[3] = new Vector2(currentWidth, currentHeight);
            newUVs[0] = new Vector2(0, 0);
            newUVs[1] = new Vector2(currentWidth, 0);

            //Back
            newUVs[7] = new Vector2(0, 0);
            newUVs[6] = new Vector2(currentWidth, 0);
            newUVs[11] = new Vector2(0, currentHeight);
            newUVs[10] = new Vector2(currentWidth, currentHeight);

            //Left
            newUVs[19] = new Vector2(currentDepth, 0);
            newUVs[17] = new Vector2(0, currentHeight);
            newUVs[16] = new Vector2(0, 0);
            newUVs[18] = new Vector2(currentDepth, currentHeight);

            //Right
            newUVs[23] = new Vector2(currentDepth, 0);
            newUVs[21] = new Vector2(0, currentHeight);
            newUVs[20] = new Vector2(0, 0);
            newUVs[22] = new Vector2(currentDepth, currentHeight);

            //Top
            newUVs[4] = new Vector2(currentWidth, 0);
            newUVs[5] = new Vector2(0, 0);
            newUVs[8] = new Vector2(currentWidth, currentDepth);
            newUVs[9] = new Vector2(0, currentDepth);

            //Bottom
            newUVs[13] = new Vector2(currentWidth, 0);
            newUVs[14] = new Vector2(0, 0);
            newUVs[12] = new Vector2(currentWidth, currentDepth);
            newUVs[15] = new Vector2(0, currentDepth);

            currentMesh.uv = newUVs;
        }
    }

    public void FixUVs()
    {
        foreach(Transform child in transform)
        {
            FixWallUVs(child);
        }
    }

    public void FindNeighbours()
    {
        if (myY > 0)
        {
            neighbourN = MazeGenerator.piecesArray[myX][myY - 1];
        }

        if (myX < maxX)
        {
            neighbourE = MazeGenerator.piecesArray[myX + 1][myY];
        }

        if (myY < maxY)
        {
            neighbourS = MazeGenerator.piecesArray[myX][myY + 1];
        }

        if (myX > 0)
        {
            neighbourW = MazeGenerator.piecesArray[myX - 1][myY];
        }
    }

    public Transform[] GetUnvisitedNeighbours()
    {
        List<Transform> unvisitedList = new List<Transform>();

        if (neighbourN && neighbourN.parent == MazeGenerator.unvisited)
        {
            unvisitedList.Add(neighbourN);
        }

        if (neighbourE && neighbourE.parent == MazeGenerator.unvisited)
        {
            unvisitedList.Add(neighbourE);
        }

        if (neighbourS && neighbourS.parent == MazeGenerator.unvisited)
        {
            unvisitedList.Add(neighbourS);
        }

        if (neighbourW && neighbourW.parent == MazeGenerator.unvisited)
        {
            unvisitedList.Add(neighbourW);
        }

        return unvisitedList.ToArray();
    }

    public void JoinToNeighbour(Transform neighbour)
    {
        if (neighbour == neighbourN)
        {
            Destroy(wallN);
            Destroy(neighbour.GetComponent<Walls>().wallS);
        }
        else if (neighbour == neighbourE)
        {
            Destroy(wallE);
            Destroy(neighbour.GetComponent<Walls>().wallW);
        }
        else if (neighbour == neighbourS)
        {
            Destroy(wallS);
            Destroy(neighbour.GetComponent<Walls>().wallN);
        }
        else if (neighbour == neighbourW)
        {
            Destroy(wallW);
            Destroy(neighbour.GetComponent<Walls>().wallE);
        }
    }

    public void FuseNorthWallsEastward()
    {
        if (wallN)
        {
            Walls currentWallsPiece = null;

            int wallLength = 1;

            if (neighbourE)
            {
                currentWallsPiece = neighbourE.GetComponent<Walls>();
            }

            while (currentWallsPiece && currentWallsPiece.wallN)
            {
                wallLength++;
                Destroy(currentWallsPiece.wallN);

                if (currentWallsPiece.neighbourE)
                {
                    currentWallsPiece = currentWallsPiece.neighbourE.GetComponent<Walls>();
                }
                else
                {
                    currentWallsPiece = null;
                }
            }

            if (wallLength > 1)
            {
                Vector3 oldScale = wallN.transform.localScale;
                Vector3 oldPosition = wallN.transform.localPosition;
                Vector3 newScale= new Vector3((oldScale.x - 1) * wallLength + 1, oldScale.y, oldScale.z);
                Vector3 newPosition = new Vector3(oldPosition.x + 4.5F * (wallLength - 1), oldPosition.y, oldPosition.z);
                wallN.transform.localScale = newScale;
                wallN.transform.localPosition = newPosition;
                FixWallUVs(wallN.transform);
            }

            wallN.transform.parent = transform.parent;
        }
    }

    public void FuseEastWallsSouthward()
    {
        if (wallE)
        {
            Walls currentWallsPiece = null;

            int wallLength = 1;

            if (neighbourS)
            {
                currentWallsPiece = neighbourS.GetComponent<Walls>();
            }

            while (currentWallsPiece && currentWallsPiece.wallE)
            {
                wallLength++;
                Destroy(currentWallsPiece.wallE);

                if (currentWallsPiece.neighbourS)
                {
                    currentWallsPiece = currentWallsPiece.neighbourS.GetComponent<Walls>();
                }
                else
                {
                    currentWallsPiece = null;
                }
            }

            if (wallLength > 1)
            {
                Vector3 oldScale = wallE.transform.localScale;
                Vector3 oldPosition = wallE.transform.localPosition;
                Vector3 newScale = new Vector3(oldScale.x, oldScale.y, (oldScale.z - 1) * wallLength + 1);
                Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z - 4.5F * (wallLength - 1));
                wallE.transform.localScale = newScale;
                wallE.transform.localPosition = newPosition;
                FixWallUVs(wallE.transform);
            }

            wallE.transform.parent = transform.parent;
        }
    }

    public void FuseSouthWallsEastward()
    {
        if (wallS)
        {
            Walls currentWallsPiece = null;

            int wallLength = 1;

            if (neighbourE)
            {
                currentWallsPiece = neighbourE.GetComponent<Walls>();
            }

            while (currentWallsPiece && currentWallsPiece.wallS)
            {
                wallLength++;
                Destroy(currentWallsPiece.wallS);

                if (currentWallsPiece.neighbourE)
                {
                    currentWallsPiece = currentWallsPiece.neighbourE.GetComponent<Walls>();
                }
                else
                {
                    currentWallsPiece = null;
                }
            }

            if (wallLength > 1)
            {
                Vector3 oldScale = wallS.transform.localScale;
                Vector3 oldPosition = wallS.transform.localPosition;
                Vector3 newScale = new Vector3((oldScale.x - 1) * wallLength + 1, oldScale.y, oldScale.z);
                Vector3 newPosition = new Vector3(oldPosition.x + 4.5F * (wallLength - 1), oldPosition.y, oldPosition.z);
                wallS.transform.localScale = newScale;
                wallS.transform.localPosition = newPosition;
                FixWallUVs(wallS.transform);
            }

            wallS.transform.parent = transform.parent;
        }
    }

    public void FuseWestWallsSouthward()
    {
        if (wallW)
        {
            Walls currentWallsPiece = null;

            int wallLength = 1;

            if (neighbourS)
            {
                currentWallsPiece = neighbourS.GetComponent<Walls>();
            }

            while (currentWallsPiece && currentWallsPiece.wallW)
            {
                wallLength++;
                Destroy(currentWallsPiece.wallW);

                if (currentWallsPiece.neighbourS)
                {
                    currentWallsPiece = currentWallsPiece.neighbourS.GetComponent<Walls>();
                }
                else
                {
                    currentWallsPiece = null;
                }
            }

            if (wallLength > 1)
            {
                Vector3 oldScale = wallW.transform.localScale;
                Vector3 oldPosition = wallW.transform.localPosition;
                Vector3 newScale = new Vector3(oldScale.x, oldScale.y, (oldScale.z - 1) * wallLength + 1);
                Vector3 newPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z - 4.5F * (wallLength - 1));
                wallW.transform.localScale = newScale;
                wallW.transform.localPosition = newPosition;
                FixWallUVs(wallW.transform);
            }

            wallW.transform.parent = transform.parent;
        }
    }
}