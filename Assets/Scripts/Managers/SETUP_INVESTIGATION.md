# Système d'Investigation des Objets Disparus

## Aperçu
Ce système automatise l'investigation des NPCs quand un objet portable disparaît ou est jeté hors de sa salle d'origine.

## Architecture

### 1. **RoomManager** (Singleton)
- Détecte automatiquement toutes les salles (`Room_*` et `Salon_*`)
- Détermine dans quelle salle se trouve un objet
- À mettre sur un GameObject vide dans la scène

### 2. **ObjectMonitor** (À ajouter à chaque objet portable)
- Sauvegarde la position initiale au démarrage
- Détecte si l'objet a bougé (délai de 1.5s configurable)
- Notifie `PNJ_Investigation` quand l'objet change

### 3. **PNJ_Investigation** (À ajouter au GameObject du NPC avec PatrouilleBoucle)
- Reçoit les ordres d'investigation de `ObjectMonitor`
- Vérifie que le joueur n'est pas dans la salle
- Se dirige vers la salle, attend 3 secondes
- Réinitialise l'objet et reprend la patrouille

### 4. **PatrouilleBoucle** (Modifié)
- Fonctionne normalement avec les waypoints
- Peut être pausé/repris par `PNJ_Investigation`

## Configuration en 5 étapes

### Étape 1 : Ajouter RoomManager à la scène
```
1. Créer un GameObject vide nommé "RoomManager"
2. Ajouter le script RoomManager.cs
3. C'est tout ! Il trouvera toutes les salles automatiquement
```

### Étape 2 : Configurer les salles
```
Assurez-vous que :
- Chaque salle a un Collider avec "Is Trigger" = ON
- Le nom commence par "Room_" ou "Salon_" (ex: Room_1, Salon_Kitchen)
- Le Rigidbody est en mode Kinematic
```

### Étape 3 : Ajouter ObjectMonitor aux objets
```
1. Sélectionner l'objet portable (doit avoir ObjetPortable.cs)
2. Ajouter le composant ObjectMonitor.cs
3. Les paramètres par défaut conviennent (délai: 1.5s)
```

### Étape 4 : Configurer le NPC
```
1. Le NPC doit avoir :
   - NavMeshAgent
   - Rigidbody (Kinematic)
   - PatrouilleBoucle.cs avec des waypoints
   - PNJ_Investigation.cs (ajouter le composant)
   
2. Assurer que le NavMesh couvre toutes les salles
```

### Étape 5 : Vérifier les Tags
```
- Le joueur doit avoir le tag "Player"
- Les NPCs ne doivent PAS avoir ce tag
```

## Flux d'exécution

```
1. Joueur prend objet → ObjetPortable.Ramasser()
2. Joueur jette objet → ObjetPortable.Poser()
3. ObjectMonitor détecte le changement → Attent 1.5s
4. ObjectMonitor → appelle PNJ_Investigation.InvestigateRoom()
5. PNJ_Investigation :
   - Vérifie que joueur ≠ salle de l'objet
   - PatrouilleBoucle.enabled = false
   - Agent se dirige vers la salle
   - Attend 3 secondes dans la salle
   - ObjectMonitor.ResetToInitialPosition()
   - PatrouilleBoucle.enabled = true
```

## Paramètres configurables

### ObjectMonitor
- **detectionDelay** : Délai avant investigation (défaut: 1.5s)
- **positionTolerance** : Distance min pour considérer comme "bougé" (défaut: 0.1m)

### PNJ_Investigation
- **investigationDuration** : Temps d'investigation dans la salle (défaut: 3s)

## Debugging

Tous les scripts utilisent `Debug.Log()` avec préfixes :
- `[RoomManager]` - Salles détectées
- `[ObjectMonitor]` - Changements d'objets
- `[PNJ_Investigation]` - Étapes d'investigation

Ouvrir la Console (Ctrl+Shift+C) pour voir le flux en temps réel.

## Exemple de hiérarchie

```
Scene/
├── RoomManager (vide + RoomManager.cs)
├── NPC
│   ├── NavMeshAgent
│   ├── PatrouilleBoucle (avec WP_1, WP_2, ...)
│   └── PNJ_Investigation
├── Room_1 (Cube + Collider Trigger)
├── Room_2 (Cube + Collider Trigger)
├── Salon_Kitchen (Cube + Collider Trigger)
├── Vase (ObjetPortable + ObjectMonitor)
├── Sofa (ObjetPortable + ObjectMonitor)
└── Player
```

## Cas limites

- **Objet jeté HORS de toutes les salles** : Le NPC n'investigue pas (comportement correct)
- **Joueur dans la salle lors du changement** : Le NPC n'investigue pas, jauge augmente ✓
- **Plusieurs objets changent** : Chacun trigger une investigation (actuellement séquentielle, amélioration possible)

## Amélioration futures possibles

- [ ] Queue d'investigation si plusieurs objets changent
- [ ] Animations/sons lors de l'investigation
- [ ] Notifier le joueur que le NPC a investigué
- [ ] Pénalité de score si objet réinitialisé
