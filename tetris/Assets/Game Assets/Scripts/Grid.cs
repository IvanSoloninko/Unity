using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Цей клас обробляє всю логіку щодо видалення рядків, коли користувач
// отримано повний рядок з блоками. А також властивість Grid.grid
// зберігає на матриці стан гри, якщо в заданому положенні (x, y)
// є блок чи ні.

public class Grid : MonoBehaviour {

    // Сама сітка
    public static int w = 10;
    public static int h = 20;
   // сітка, що зберігає елемент Transform
    public static Transform[,] grid = new Transform[w, h];

  // перетворюємо дійсний вектор у дискретні координати за допомогою Mathf.Round
    public static Vector2 roundVector2(Vector2 v) {
        return new Vector2 (Mathf.Round (v.x), Mathf.Round (v.y));
    }

   // перевіряємо, чи є якийсь вектор у межах гри (межі ліворуч, праворуч та вниз)
    public static bool insideBorder(Vector2 pos) {
        return ((int)pos.x >= 0 &&
                (int)pos.x < w &&
                (int)pos.y >= 0);
    }

    // знищуємо рядок у рядку y
    public static void deleteRow(int y) {
        for (int x = 0; x < w; x++) {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }


   // Щоразу, коли рядок видалявся, вищезазначені рядки повинні падати внизу на одну одиницю.
     // Наступна функція подбає про це:
    public static void decreaseRow(int y) {
        for (int x = 0; x < w; x++) {
            if (grid[x, y] != null) {
               // рухаємось одним донизу
                grid[x, y - 1] = grid[x, y];
                grid[x,y] = null;

               // Оновлення позиції блоку
                grid[x, y-1].position += new Vector3(0, -1, 0);
            }
        }
    }

   // щоразу, коли рядок видаляється, усі вищезазначені рядки слід зменшувати на 1
    public static void decreaseRowAbove(int y) {
        for (int i = y; i < h; i++) {
            decreaseRow(i);
        }
    }

  // перевіряємо, чи рядок заповнений, а потім може бути видалений (оцінка +1)
    public static bool isRowFull(int y){
        for (int x = 0; x < w; x++) {
            if (grid[x, y] == null) {
                return false;
            }
        }
        return true;

    }

    public static void deleteFullRows() {
        for (int y = 0; y < h; y++) {
            if (isRowFull(y)) {
                deleteRow(y);
                decreaseRowAbove(y + 1);
               // додаємо нові бали, щоб набрати, коли рядок буде видалено
                ScoreManager.score += (h - y) * 10;
                --y;
              
            }
        }
    }



}
