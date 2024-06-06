using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameMain : MonoBehaviour
{
    public static GameMain Main { get; private set; }
    public Factory factory;
    public MachineProgress show;
    public MachineRecipe recipe;
    public bool isShow = false;
    public int ShowIndex = 1;

    public GameObject pre;
    private void Awake()
    {
        Main = this;
    }

    private void Start()
    {
        factory = new Factory();
        factory.Init();
        factory.pre = pre;

    }

    private void FixedUpdate()
    {
        factory.GameTick();
    }

    private void Update()
    {
        factory.ShowBelt();
        if (isShow)
        {
            
            show = factory.CopyShow(ShowIndex);
            recipe = factory.CopyShowRecipe(show.RecipeId);
        }
    }
}
