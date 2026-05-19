using UnityEngine;

public class SphereMalus : MonoBehaviour
{
    public int pv = 20;
    void OnTriggerEnter(Collider col){
        if (col.gameObject.tag == "Player"){
            HudManager hud = HudManager.instance;
            hud.subPV(20);
            hud.showTimedMessage("Vous avez perdu " + pv + " PV.");
            AudioManager am = AudioManager.instance;
            am.PlaySFX(am.sfx_list.sfx_hit);
            Destroy(this.gameObject);
        }
    }
}
