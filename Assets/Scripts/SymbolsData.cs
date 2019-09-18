using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SymbolsData :MonoBehaviour
{   
    public Symbol[,] symbols = new Symbol[5, 6];            // storing symbols data in matrix for reels -> 5 reels with six pictures
    public float startPosition = 150f;                      //top position - where we start drawing
    public float distance = 65f;                            //distance between symbols on reel
      
    private DataLoader loader ;
    public DataLoader.SlotData[] data;

    public struct Symbol
    {
        public GameObject SymbolObject;
        public int Position;
        public string SymbolName;
    }

    public static SymbolsData instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        /*loading symbols positions from Data.json*/
        loader = new DataLoader();
        data = loader.LoadData("Data");

        InstantiateSymbols();
    }

    private void InstantiateSymbols()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                /*loading prefab from resources for each symbol */
                
                symbols[i, j].SymbolObject = (GameObject)Instantiate(Resources.Load(data[i * 6 + j].name));
                /*setting name*/
                symbols[i, j].SymbolName = data[i * 6 + j].name;
                /* adding to parent reel-> indexes starting from 1 */
                GameObject parentObject = GameObject.Find(("Reel0" + (i + 1).ToString()));
                symbols[i, j].SymbolObject.transform.SetParent(parentObject.transform);

                /* changing transformation of picture on reel */
                RectTransform instanceTransformation = symbols[i, j].SymbolObject.GetComponent<RectTransform>();
                instanceTransformation.anchoredPosition = new Vector2(0, startPosition - distance * j);
                instanceTransformation.localScale = new Vector3(80, 40, 1);

                /* setting position on reel -> will be changed later */
                symbols[i, j].Position = j;

            }
        }
    }

    public void PrintSymbols()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                Debug.Log(i.ToString() + " " + j.ToString() + " : " + symbols[i, j].SymbolObject.GetComponent<RectTransform>().anchoredPosition.ToString() + " : " + symbols[i, j].SymbolObject.name);
            }
        }
    }


}
