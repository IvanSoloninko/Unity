using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Group : MonoBehaviour {

   // час останнього випадання елемента, що використовується для автоматичного падіння після
     // час, параметризований за `рівнем`
    private float lastFall;

  // час останнього натискання клавіші для обробки поведінки тривалого натискання
    private float lastKeyDown;
    private float timeKeyPressed;

    public void AlignCenter() {
        transform.position += transform.position - Utils.Center(gameObject);
    }


    bool isValidGridPos() {
        foreach (Transform child in transform) {
            Vector2 v = Grid.roundVector2(child.position);

            // не всередині кордону?
            if(!Grid.insideBorder(v)) {
                return false;
            }

           // Блок в комірці сітки ?
            if (Grid.grid[(int)(v.x), (int)(v.y)] != null &&
                Grid.grid[(int)(v.x), (int)(v.y)].parent != transform) {
                return false;
            }
        }

        return true;
    }

 // оновлення сітки
    void updateGrid() {
       // Видалити старі обєкти із сітки
        for (int y = 0; y < Grid.h; ++y) {
            for (int x = 0; x < Grid.w; ++x) {
                if (Grid.grid[x,y] != null &&
                    Grid.grid[x,y].parent == transform) {
                    Grid.grid[x,y] = null;
                }
            } 
        }

        insertOnGrid();
    }

    void insertOnGrid() {
       // додаємо нових обєктів до сітки
        foreach (Transform child in transform) {
            Vector2 v = Grid.roundVector2(child.position);
            Grid.grid[(int)v.x,(int)v.y] = child;
        }
    }

    void gameOver() {
        Debug.Log("GAME OVER!");
        while (!isValidGridPos()) {
          
            transform.position  += new Vector3(0, 1, 0);
        } 
        updateGrid(); // щоб не перекривати недійсні групи
        enabled = false; // вимкнути скрипт при смерті
        UIController.gameOver(); // активна панель Game Over
        Highscore.Set(ScoreManager.score); // встановити рекорд
        
    }

   // Використовуй це для ініціалізації
    void Start () {
        lastFall = Time.time;
        lastKeyDown = Time.time;
        timeKeyPressed = Time.time;
        if (isValidGridPos()) {
            insertOnGrid();
        } else { 
            Debug.Log("KILLED ON START");
            gameOver();
        }

    }

    void tryChangePos(Vector3 v) {
        
        
        transform.position += v;

        // See if valid
        if (isValidGridPos()) {
            updateGrid();
        } else {
            transform.position -= v;
        }
    }

    void fallGroup() {
       // модифікувати
        transform.position += new Vector3(0, -1, 0);

        if (isValidGridPos()){
           // Дійсний. Оновити сітку ... ще раз
            updateGrid();
        } else {
           // це недійсне. повернути
            transform.position += new Vector3(0, 1, 0);

            // Очистити заповнені горизонтальні лінії
            Grid.deleteFullRows();

            // Вимкнути сценарій
            FindObjectOfType<Spawner>().spawnNext();


            
            enabled = false;
        }

        lastFall = Time.time;

    }

   // getKey, якщо натиснути зараз на довше натиснути на 0,5 секунди | якщо це істина, застосовуйте клавішу кожного 0,05f під час натискання
    bool getKey(KeyCode key) {
        bool keyDown = Input.GetKeyDown(key);
        bool pressed = Input.GetKey(key) && Time.time - lastKeyDown > 0.5f && Time.time - timeKeyPressed > 0.05f;

        if (keyDown) {
            lastKeyDown = Time.time;
        }
        if (pressed) {
            timeKeyPressed = Time.time;
        }
 
        return keyDown || pressed;
    }


   // Оновлення викликається один раз на кадр
    void Update () {
        if (UIController.isPaused) {
            return; // нічого не робити
        }
        if (getKey(KeyCode.LeftArrow)) {
            tryChangePos(new Vector3(-1, 0, 0));
        } else if (getKey(KeyCode.RightArrow)) {  // Рухатися вправо
            tryChangePos(new Vector3(1, 0, 0));
        } else if (getKey(KeyCode.UpArrow) && gameObject.tag != "Cube") { // Обертати
            transform.Rotate(0, 0, -90);

         // перевірити, чи дійсно
            if (isValidGridPos()) {
             // він дійсний. Оновити сітку
                updateGrid();
            } else {
               // це недійсне. повернути
                transform.Rotate(0, 0, 90);
            }
        } else if (getKey(KeyCode.DownArrow) || (Time.time - lastFall) >= (float)1 / Mathf.Sqrt(LevelManager.level)) {
            fallGroup();
        } else if (Input.GetKeyDown(KeyCode.Space)) {
            while (enabled) {  // падати до дна
                fallGroup();
            }
        }

    }
}
