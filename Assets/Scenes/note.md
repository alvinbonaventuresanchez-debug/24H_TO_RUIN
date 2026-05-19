# README - Scènes Unity

## Liste des scènes

```txt id="yo4i8y"
Scenes/
├── MainMenu.unity
├── Gameplay.unity
├── Victory.unity
├── Defeat.unity
├── Leaderboard.unity
├── Credits.unity
└── Loading.unity
```

---

# Description des scènes

## MainMenu

Menu principal du jeu.

Contient :

* bouton Jouer
* accès leaderboard
* accès crédits
* paramètres
* sélection langue

---

## Gameplay

Scène principale du jeu.

Contient :

* gameplay joueur
* HUD
* timer
* système de score
* système de suspicion
* pause menu

---

## Victory

Scène affichée lorsque le joueur gagne.

Contient :

* score final
* résumé de partie
* recommencer
* retour menu

---

## Defeat

Scène affichée lorsque le joueur perd.

Conditions possibles :

* timer terminé
* suspicion maximale
* échec mission

Contient :

* score
* raison de défaite
* recommencer
* retour menu

---

## Leaderboard

Affichage :

* meilleurs scores
* scores joueur

---

## Credits

Affiche :

* membres du projet
* rôles
* ressources utilisées

---

## Loading

Scène optionnelle.

Utilisée pour :

* transitions
* chargement asynchrone
* préchargement ressources

---

# Notes

Les éléments suivants ne sont pas des scènes séparées :

* pause menu
* paramètres
* langues

Ils sont gérés directement en UI via des panels Canvas.
