using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Collections;

public class SineLineGenerator : MonoBehaviour {

    public GlobalController Control;
    public float amplitude;
    public Vector2 SinClamp;
    public List<Transform> Points;
    public Transform PointPrefab;
    public Transform DustPrefab, SchroomPrefab, PillPrefab, CarpPrefab;
    public Transform[] FoodPrefab;

    public PlayerMechanics PlayerScript;
    public BigFishScript BigFish;

    public AudioMixer PitchedAudio;

    public bool createEnemies, gameStarted;
    public GUITexture WhiteFlash;

    public AudioMaster MusicManager;

    void Start()
    {
        amplitude = 1;
        Points = new List<Transform>();
        Vector3 StartPos = new Vector3(-11F, 0F, 1F);
        for(int i = 0; i < 45; i++)
        {
            Points.Add(Instantiate(PointPrefab, StartPos, Quaternion.identity) as Transform);
            Points[i].parent = transform;
            Points[i].name = "SinePoint " + (i + 1);
            StartPos = new Vector3(StartPos.x + 0.5F, Mathf.Sin((i+1) / 2F) * amplitude, 1F);
        }
    }

    public IEnumerator SetPointsAtStart()
    {
        StartCoroutine(MusicManager.FadeNextTrack(0F));
        Control = (GlobalController)FindObjectOfType(typeof(GlobalController));
        Control.globalSpeed = 5F;

        StartCoroutine(WhiteScreenKick());

        PlayerScript.PointHolded = Points[8];
        Points[8].GetComponent<SpriteRenderer>().enabled = false;
        PlayerScript.PointNext = Points[9];
        PlayerScript.enabled = true;

        for (int i = 13; i < 20; i++)
        {
            Points[i].GetComponent<SpriteRenderer>().color = new Color(1F, 1F, 1F, (109F / 255F) / ((i - 11) / 2F));
        }

        for (int i = 20; i < 44; i++)
        {
            Destroy(Points[20].gameObject);
            Points.Remove(Points[20]);
        }

        yield return new WaitForSeconds(9F);
        PlayerScript.energy = PlayerScript.energyMax;

        Invoke("CreateFood", Random.Range(1F, 3F));
        BigFish.enabled = true;
    }

	void Update ()
    {
        MovePoints();

        PitchedAudio.SetFloat("Pitch", 2F-((Mathf.Abs(amplitude)+1F)/4F));

        if (Input.GetKey("up") && amplitude < SinClamp.x)
            amplitude += 5F * (Control.globalSpeed / 5F) * Time.deltaTime;
        if (Input.GetKey("down") && amplitude > SinClamp.y)
            amplitude -= 5F * (Control.globalSpeed / 5F) * Time.deltaTime;
    }

    void MovePoints()
    {
        for (int index = 0; index < Points.Count; index++)
            Points[index].position = new Vector3(Points[index].position.x, Mathf.Sin(Points[index].position.x + Time.time * Control.startGlobalSpeed) * amplitude, 1F);
    }

    void CreateFood()
    {
        Transform newFood = Instantiate(FoodPrefab[Control.currentLevel], new Vector3(Points[Points.Count-1].position.x, Points[Points.Count - 1].position.y, 0.2F), FoodPrefab[Control.currentLevel].rotation) as Transform;
        float period = Mathf.Asin(Points[Points.Count - 1].position.y / ((amplitude == 0) ? 0.01F : amplitude));
        float min = Mathf.Sin(period) * SinClamp.y;
        float max = Mathf.Sin(period) * SinClamp.x;
        newFood.GetComponent<FoodScript>().SetStartY(Random.Range(min, max));
        Invoke("CreateFood", Random.Range(1F, 3F));
    }

    public void CreateDust()
    {
        Transform newFood = Instantiate(DustPrefab, new Vector3(Points[Points.Count - 1].position.x, Points[Points.Count - 1].position.y, 0F), DustPrefab.rotation) as Transform;
        newFood.GetComponent<FoodScript>().SetStartY(0F);
        Invoke("CreateFood",5F);
        Invoke("CreateCarp", 5F);
        StartCoroutine(MusicManager.FadeNextTrack(4F));
    }

    public void CreateSchroom()
    {
        Transform newFood = Instantiate(SchroomPrefab, new Vector3(Points[Points.Count - 1].position.x, Points[Points.Count - 1].position.y, 0F), SchroomPrefab.rotation) as Transform;
        newFood.GetComponent<FoodScript>().SetStartY(0F);
        Invoke("CreateFood", 5F);
        StartCoroutine(MusicManager.FadeNextTrack(4F));
    }

    public void CreatePill()
    {
        Transform newFood = Instantiate(PillPrefab, new Vector3(Points[Points.Count - 1].position.x, Points[Points.Count - 1].position.y, 0F), PillPrefab.rotation) as Transform;
        newFood.GetComponent<FoodScript>().SetStartY(0F);
        Invoke("CreateFood", 5F);
        StartCoroutine(MusicManager.FadeNextTrack(4F));
    }

    void CreateCarp()
    {
        Transform newCarp = Instantiate(CarpPrefab, new Vector3(Points[Points.Count - 1].position.x, Points[Points.Count - 1].position.y, 0.1F), CarpPrefab.rotation) as Transform;
        float rnd = Random.Range(0.5F, 2F) * ((Random.Range(0, 2) == 0) ? -1 : 1);
        newCarp.GetComponent<FoodScript>().SetStartY(rnd);
        Invoke("CreateCarp", Random.Range(4F, 6F));
    }

    IEnumerator WhiteScreenKick()
    {
        WhiteFlash.color = new Color(1F, 1F, 1F, 0.5F);
        while(WhiteFlash.color.a > 0F)
        {
            WhiteFlash.color -= new Color(0F, 0F, 0F, 1F * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        WhiteFlash.color = new Color(1F, 1F, 1F, 0F);
    }

    public IEnumerator BlackScreenDarken()
    {
        yield return new WaitForSeconds(3F);
        WhiteFlash.color = new Color(0F, 0F, 0F, 0F);
        while (WhiteFlash.color.a< 1F)
        {
            WhiteFlash.color += new Color(0F, 0F, 0F, 0.3F * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        WhiteFlash.color = new Color(0F, 0F, 0F, 1F);
        Application.LoadLevel(0);
    }
}
