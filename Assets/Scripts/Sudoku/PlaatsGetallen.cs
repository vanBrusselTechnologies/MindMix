using System.Collections.Generic;
using UnityEngine;

public class PlaatsGetallen : MonoBehaviour
{
    public bool NormaalGetal = true;
    private int een = 1;
    private int twee = 2;
    private int drie = 3;
    private int vier = 4;
    private int vijf = 5;
    private int zes = 6;
    private int zeven = 7;
    private int acht = 8;
    private int negen = 9;
    private List<int> tot9 = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    [HideInInspector] public List<int> allCijfers;
    private SaveScript saveScript;

    public void Start0()
    {
        saveScript = GameObject.Find("gegevensHouder").GetComponent<SaveScript>();
        List<int> randTot9 = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (int i = 0; i < tot9.Count; i++)
        {
            tot9[i] = randTot9[Random.Range(0, randTot9.Count)];
            randTot9.Remove(tot9[i]);
        }
        een = tot9[0];
        twee = tot9[1];
        drie = tot9[2];
        vier = tot9[3];
        vijf = tot9[4];
        zes = tot9[5];
        zeven = tot9[6];
        acht = tot9[7];
        negen = tot9[8];
        int welkeGetallenVolgorde = Random.Range(0, 1);
        if (welkeGetallenVolgorde == 0) allCijfers = new List<int> { zes, drie, negen, vier, zeven, acht, twee, vijf, een, vijf, een, vier, twee, zes, drie, acht, zeven, negen, zeven, twee, acht, negen, een, vijf, drie, vier, zes, zeven, vier, zes, negen, twee, vijf, een, acht, drie, drie, negen, vijf, een, vier, acht, zeven, twee, zes, twee, acht, een, zes, zeven, drie, vier, vijf, negen, vijf, een, vier, drie, zes, zeven, acht, negen, twee, negen, drie, twee, vier, acht, een, zes, vijf, zeven, acht, zes, zeven, vijf, negen, twee, een, drie, vier };
        else if (welkeGetallenVolgorde == 1) allCijfers = new List<int> { zeven, zes, acht, drie, twee, een, vier, negen, vijf, twee, negen, vijf, acht, vier, zes, zeven, een, drie, een, vier, drie, vijf, zeven, negen, zes, acht, twee, twee, zeven, zes, vijf, een, negen, acht, vier, drie, negen, acht, een, drie, zeven, vier, zes, vijf, twee, drie, vijf, vier, twee, zes, acht, negen, een, zeven, zes, vijf, twee, een, acht, zeven, negen, drie, vier, vier, drie, acht, vijf, twee, negen, een, zes, zeven, zeven, negen, een, vier, drie, zes, acht, twee, vijf };
        for (int x = 0; x < 2; x++)
        {
            for (int a = 1; a < 4; a++)
            {
                int randNum = Random.Range(0, 5);
                if (randNum == 0)
                {
                    List<int> tempWissel1 = KrijgGetallenTot81(1, 0, a, a);
                    List<int> tempWissel2 = KrijgGetallenTot81(2, 0, a, a);
                    List<int> tempWissel1a = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < 9; i++)
                    {
                        tempWissel1a[i] = allCijfers[tempWissel1[i]];
                        allCijfers[tempWissel1[i]] = allCijfers[tempWissel2[i]];
                        allCijfers[tempWissel2[i]] = tempWissel1a[i];
                    }
                }
                else if (randNum == 1)
                {
                    List<int> tempWissel1 = KrijgGetallenTot81(1, 0, a, a);
                    List<int> tempWissel2 = KrijgGetallenTot81(3, 0, a, a);
                    List<int> tempWissel1a = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < 9; i++)
                    {
                        tempWissel1a[i] = allCijfers[tempWissel1[i]];
                        allCijfers[tempWissel1[i]] = allCijfers[tempWissel2[i]];
                        allCijfers[tempWissel2[i]] = tempWissel1a[i];
                    }
                }
                else if (randNum == 2)
                {
                    List<int> tempWissel1 = KrijgGetallenTot81(2, 0, a, a);
                    List<int> tempWissel2 = KrijgGetallenTot81(3, 0, a, a);
                    List<int> tempWissel1a = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < 9; i++)
                    {
                        tempWissel1a[i] = allCijfers[tempWissel1[i]];
                        allCijfers[tempWissel1[i]] = allCijfers[tempWissel2[i]];
                        allCijfers[tempWissel2[i]] = tempWissel1a[i];
                    }
                }
                else if (randNum == 3)
                {
                    List<int> tempWissel1 = KrijgGetallenTot81(1, 0, a, a);
                    List<int> tempWissel2 = KrijgGetallenTot81(2, 0, a, a);
                    List<int> tempWissel3 = KrijgGetallenTot81(3, 0, a, a);
                    List<int> tempWissel1a = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    List<int> tempWissel2a = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < 9; i++)
                    {
                        tempWissel1a[i] = allCijfers[tempWissel1[i]];
                        tempWissel2a[i] = allCijfers[tempWissel2[i]];
                        allCijfers[tempWissel1[i]] = allCijfers[tempWissel3[i]];
                        allCijfers[tempWissel2[i]] = tempWissel1a[i];
                        allCijfers[tempWissel3[i]] = tempWissel2a[i];
                    }
                }
            }
            for (int a = 1; a < 4; a++)
            {
                int randNum = Random.Range(0, 5);
                if (randNum == 0)
                {
                    List<int> tempWissel1 = KrijgGetallenTot81(0, 1, a, a);
                    List<int> tempWissel2 = KrijgGetallenTot81(0, 2, a, a);
                    List<int> tempWissel1a = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < 9; i++)
                    {
                        tempWissel1a[i] = allCijfers[tempWissel1[i]];
                        allCijfers[tempWissel1[i]] = allCijfers[tempWissel2[i]];
                        allCijfers[tempWissel2[i]] = tempWissel1a[i];
                    }
                }
                else if (randNum == 1)
                {
                    List<int> tempWissel1 = KrijgGetallenTot81(0, 1, a, a);
                    List<int> tempWissel2 = KrijgGetallenTot81(0, 3, a, a);
                    List<int> tempWissel1a = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < 9; i++)
                    {
                        tempWissel1a[i] = allCijfers[tempWissel1[i]];
                        allCijfers[tempWissel1[i]] = allCijfers[tempWissel2[i]];
                        allCijfers[tempWissel2[i]] = tempWissel1a[i];
                    }
                }
                else if (randNum == 2)
                {
                    List<int> tempWissel1 = KrijgGetallenTot81(0, 2, a, a);
                    List<int> tempWissel2 = KrijgGetallenTot81(0, 3, a, a);
                    List<int> tempWissel1a = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < 9; i++)
                    {
                        tempWissel1a[i] = allCijfers[tempWissel1[i]];
                        allCijfers[tempWissel1[i]] = allCijfers[tempWissel2[i]];
                        allCijfers[tempWissel2[i]] = tempWissel1a[i];
                    }
                }
                else if (randNum == 3)
                {
                    List<int> tempWissel1 = KrijgGetallenTot81(0, 1, a, a);
                    List<int> tempWissel2 = KrijgGetallenTot81(0, 2, a, a);
                    List<int> tempWissel3 = KrijgGetallenTot81(0, 3, a, a);
                    List<int> tempWissel1a = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    List<int> tempWissel2a = new() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    for (int i = 0; i < 9; i++)
                    {
                        tempWissel1a[i] = allCijfers[tempWissel1[i]];
                        tempWissel2a[i] = allCijfers[tempWissel2[i]];
                        allCijfers[tempWissel1[i]] = allCijfers[tempWissel3[i]];
                        allCijfers[tempWissel2[i]] = tempWissel1a[i];
                        allCijfers[tempWissel3[i]] = tempWissel2a[i];
                    }
                }
            }
        }
        for (int i = 0; i < allCijfers.Count; i++)
        {
            saveScript.intDict["kloppendCijferBijInt" + i] = allCijfers[i];
        }
    }

    public List<int> KrijgGetallenTot81(int kolom, int rij, int kolomGroep, int rijGroep)
    {
        if (kolom != 0)
        {
            kolom += (kolomGroep - 1) * 3;
        }
        if (rij != 0)
        {
            rij += (rijGroep - 1) * 3;
        }
        List<int> getallenTot81 = new();
        if (kolom == 0 && rij != 0)
        {
            if (rij == 1)
            {
                getallenTot81.Add(10); //Middenboven
                getallenTot81.Add(11);
                getallenTot81.Add(12);
                getallenTot81.Add(19); //Rechtsboven
                getallenTot81.Add(20);
                getallenTot81.Add(21);
                getallenTot81.Add(1); //Linksboven
                getallenTot81.Add(2);
                getallenTot81.Add(3);
            }
            else if (rij == 2)
            {
                getallenTot81.Add(13); //Middenboven
                getallenTot81.Add(14);
                getallenTot81.Add(15);
                getallenTot81.Add(22); //Rechtsboven
                getallenTot81.Add(23);
                getallenTot81.Add(24);
                getallenTot81.Add(4); //Linksboven
                getallenTot81.Add(5);
                getallenTot81.Add(6);
            }
            else if (rij == 3)
            {
                getallenTot81.Add(16); //Middenboven
                getallenTot81.Add(17);
                getallenTot81.Add(18);
                getallenTot81.Add(25); //Rechtsboven
                getallenTot81.Add(26);
                getallenTot81.Add(27);
                getallenTot81.Add(7); //Linksboven
                getallenTot81.Add(8);
                getallenTot81.Add(9);
            }
            else if (rij == 4)
            {
                getallenTot81.Add(37); //Midden
                getallenTot81.Add(38);
                getallenTot81.Add(39);
                getallenTot81.Add(28); //Linksmidden
                getallenTot81.Add(29);
                getallenTot81.Add(30);
                getallenTot81.Add(46); //Rechtsmidden
                getallenTot81.Add(47);
                getallenTot81.Add(48);
            }
            else if (rij == 5)
            {
                getallenTot81.Add(40); //Midden
                getallenTot81.Add(41);
                getallenTot81.Add(42);
                getallenTot81.Add(31); //Linksmidden
                getallenTot81.Add(32);
                getallenTot81.Add(33);
                getallenTot81.Add(49); //Rechtsmidden
                getallenTot81.Add(50);
                getallenTot81.Add(51);
            }
            else if (rij == 6)
            {
                getallenTot81.Add(43); //Midden
                getallenTot81.Add(44);
                getallenTot81.Add(45);
                getallenTot81.Add(34); //Linksmidden
                getallenTot81.Add(35);
                getallenTot81.Add(36);
                getallenTot81.Add(52); //Rechtsmidden
                getallenTot81.Add(53);
                getallenTot81.Add(54);
            }
            else if (rij == 7)
            {
                getallenTot81.Add(55); //Linksonder
                getallenTot81.Add(56);
                getallenTot81.Add(57);
                getallenTot81.Add(73); //Rechtsonder
                getallenTot81.Add(74);
                getallenTot81.Add(75);
                getallenTot81.Add(64); //Middenonder
                getallenTot81.Add(65);
                getallenTot81.Add(66);
            }
            else if (rij == 8)
            {
                getallenTot81.Add(58); //Linksonder
                getallenTot81.Add(59);
                getallenTot81.Add(60);
                getallenTot81.Add(76); //Rechtsonder
                getallenTot81.Add(77);
                getallenTot81.Add(78);
                getallenTot81.Add(67); //Middenonder
                getallenTot81.Add(68);
                getallenTot81.Add(69);
            }
            else if (rij == 9)
            {
                getallenTot81.Add(61); //Linksonder
                getallenTot81.Add(62);
                getallenTot81.Add(63);
                getallenTot81.Add(79); //Rechtsonder
                getallenTot81.Add(80);
                getallenTot81.Add(81);
                getallenTot81.Add(70); //Middenonder
                getallenTot81.Add(71);
                getallenTot81.Add(72);
            }
        }
        else if (kolom != 0 && rij == 0)
        {
            if (kolom == 1)
            {
                getallenTot81.Add(1); //Linksboven
                getallenTot81.Add(4);
                getallenTot81.Add(7);
                getallenTot81.Add(28); //Linksmidden
                getallenTot81.Add(31);
                getallenTot81.Add(34);
                getallenTot81.Add(55); //Linksonder
                getallenTot81.Add(58);
                getallenTot81.Add(61);
            }
            else if (kolom == 2)
            {
                getallenTot81.Add(2); //Linksboven
                getallenTot81.Add(5);
                getallenTot81.Add(8);
                getallenTot81.Add(29); //linksmidden
                getallenTot81.Add(32);
                getallenTot81.Add(35);
                getallenTot81.Add(56); //linksonder
                getallenTot81.Add(59);
                getallenTot81.Add(62);
            }
            else if (kolom == 3)
            {
                getallenTot81.Add(3); //Linksboven
                getallenTot81.Add(6);
                getallenTot81.Add(9);
                getallenTot81.Add(30); //linksmidden
                getallenTot81.Add(33);
                getallenTot81.Add(36);
                getallenTot81.Add(57); //Linksonder
                getallenTot81.Add(60);
                getallenTot81.Add(63);
            }
            else if (kolom == 4)
            {
                getallenTot81.Add(10); //middenboven
                getallenTot81.Add(13);
                getallenTot81.Add(16);
                getallenTot81.Add(37); //midden
                getallenTot81.Add(40);
                getallenTot81.Add(43);
                getallenTot81.Add(64); //middenonder
                getallenTot81.Add(67);
                getallenTot81.Add(70);
            }
            else if (kolom == 5)
            {
                getallenTot81.Add(11); //middenboven
                getallenTot81.Add(14);
                getallenTot81.Add(17);
                getallenTot81.Add(38); //midden
                getallenTot81.Add(41);
                getallenTot81.Add(44);
                getallenTot81.Add(65); //middenonder
                getallenTot81.Add(68);
                getallenTot81.Add(71);
            }
            else if (kolom == 6)
            {
                getallenTot81.Add(12); //middenboven
                getallenTot81.Add(15);
                getallenTot81.Add(18);
                getallenTot81.Add(39); //midden
                getallenTot81.Add(42);
                getallenTot81.Add(45);
                getallenTot81.Add(66); //middenonder
                getallenTot81.Add(69);
                getallenTot81.Add(72);
            }
            else if (kolom == 7)
            {

                getallenTot81.Add(19); //rechtsboven
                getallenTot81.Add(22);
                getallenTot81.Add(25);
                getallenTot81.Add(46); //rechtsmidden
                getallenTot81.Add(49);
                getallenTot81.Add(52);
                getallenTot81.Add(73); //rechtsonder
                getallenTot81.Add(76);
                getallenTot81.Add(79);
            }
            else if (kolom == 8)
            {
                getallenTot81.Add(20); //rechtsboven
                getallenTot81.Add(23);
                getallenTot81.Add(26);
                getallenTot81.Add(47); //rechtsmidden
                getallenTot81.Add(50);
                getallenTot81.Add(53);
                getallenTot81.Add(74); //rechtsonder
                getallenTot81.Add(77);
                getallenTot81.Add(80);
            }
            else if (kolom == 9)
            {
                getallenTot81.Add(21); //rechtsboven
                getallenTot81.Add(24);
                getallenTot81.Add(27);
                getallenTot81.Add(48); //rechtsmidden
                getallenTot81.Add(51);
                getallenTot81.Add(54);
                getallenTot81.Add(75); //rechtsonder
                getallenTot81.Add(78);
                getallenTot81.Add(81);
            }
        }
        getallenTot81.Sort();
        for (int i = 0; i < getallenTot81.Count; i++)
        {
            getallenTot81[i] = getallenTot81[i] - 1;
        }
        return getallenTot81;
    }
}