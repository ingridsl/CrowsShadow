using System.Collections.Generic;

[System.Serializable]
public class Save
{
    public string time;

    // MISSÕES
    public int mission = 0; // atual

    // INVENTÁRIO
    public List<CrowShadowPlayer.Inventory.InventoryItems> inventory = new List<CrowShadowPlayer.Inventory.InventoryItems>(); // completo
    public int currentItem = -1; // item atual
    public int lifeTampa = 80; // vida restante para objeto protetor
    public int lifeEscudo = 120; // vida restante para objeto protetor
    public int pedraCount = 0;
    public int papelCount = 0;

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
    public bool mission10BurnCorredor = false; // M8: queimou o corredor ou não - quarto da mãe (caminho mal) -> M10
}