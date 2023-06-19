using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;

public class MovieListDisplay : MonoBehaviour
{
    public GameObject movieItemPrefab;
    public Transform movieListContainer;

    private const string APIKey = "00243336e2f949edba05fc655da4510e";
    private const string APIURL = "https://api.themoviedb.org/3/discover/movie?include_adult=false&include_video=false&language=en-US&page=1&sort_by=popularity.desc";

    private void Start()
    {
        StartCoroutine(GetMovieData());
    }

    private IEnumerator GetMovieData()
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(APIURL))
        {
            webRequest.SetRequestHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIwMDI0MzMzNmUyZjk0OWVkYmEwNWZjNjU1ZGE0NTEwZSIsInN1YiI6IjVhYzFjM2IxMGUwYTI2NGE1NzA1NmEwMSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.uy3Lj5gCGGhxulu3ocPzJVh10f7KE_x1IDSE16CGzKw");
            webRequest.url = APIURL + "&api_key=" + APIKey;
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
            }
            else
            {
                string response = webRequest.downloadHandler.text;
                ProcessMovieData(response);
            }
        }
    }

    private void ProcessMovieData(string data)
    {
        // Преобразование JSON-строки в объекты данных
        MovieList movieList = JsonUtility.FromJson<MovieList>(data);

        // Виведення списку фільмів
        foreach (Movie movie in movieList.results)
        {
            GameObject movieItem = Instantiate(movieItemPrefab, movieListContainer);
            MovieItemDisplay itemDisplay = movieItem.GetComponent<MovieItemDisplay>();

            // Встановлення зображення, назви та опису фільму
            StartCoroutine(SetImage(movie.backdrop_path, itemDisplay.image));
            itemDisplay.title.text = movie.title;
            itemDisplay.overview.text = movie.overview;
        }
    }

    private IEnumerator SetImage(string imagePath, Image image)
    {
        string baseUrl = "https://image.tmdb.org/t/p/w500";
        string imageUrl = baseUrl + imagePath;
        UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(imageUrl);        
            yield return imageRequest.SendWebRequest();

        if (imageRequest.result == UnityWebRequest.Result.ConnectionError || imageRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error fetching image: " + imageRequest.error);
        }
        else
        {
            Texture2D texture = DownloadHandlerTexture.GetContent(imageRequest);
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }

    }


    // Класи для десеріалізації JSON-даних
    [System.Serializable]
    private class MovieList
    {
        public Movie[] results;
    }

    [System.Serializable]
    private class Movie
    {
        public string backdrop_path;
        public string title;
        public string overview;
    }

}

