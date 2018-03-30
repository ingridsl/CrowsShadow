﻿using System.Collections.Generic;

[System.Serializable]
public class Save
{
    // MISSÕES
    public int mission = 0; // atual
    public int unlockedMission = 0; // desbloqueadas

    // INVENTÁRIO
    public List<Inventory.InventoryItems> inventory = new List<Inventory.InventoryItems>(); // completo
    public int currentItem = -1; // item atual
    public int lifeTampa = 80; // vida restante para objeto protetor

    // COLECIONÁVEIS
    public int numberPages = 0; // número de páginas encontradas

    // MISSÕES EXTRAS
    public int sideQuests = 0; // número de side questes executadas

    // ESCOLHAS
    public float pathBird = 0; // caminho bom
    public float pathCat = 0; // caminho mal

    // ESCOLHAS ESPECÍFICAS
    public bool mission1AssustaGato = false; // M1: assustou o gato ou não
    public bool mission2ContestaMae = false; // M2: brigou com a mãe ou não
    public bool mission4QuebraSozinho = false; // M4: quebrou o vazo sozinho ou não - ajuda do gato
    public bool mission8BurnCorredor = false; // M8: queimou o corredor ou não - quarto da mãe (caminho mal) -> M10
}