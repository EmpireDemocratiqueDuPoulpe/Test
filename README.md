# Résultat
Le jeu est fait pour être jouer en résolution 720 x 1280, une résolution de téléphone. D'autres résolutions peuvent fonctionner mais je ne suis pas sûr du résultat.

## Fonctionnalités
Le jeu suit les règles du Taquin. Une texture est découpée en neuf carrés et l'utilisateur doit les remettre dans l'ordre dans le temps imparti. **Un carré ne peut être déplacée que dans une place vide.**

- Il est possible de modifier les règles du jeu depuis l'Inspecteur en cliquant sur le GameObject _"**GameManager**"_.
- Il est aussi possible de modifier la configuration de la grille depuis l'Inspecteur en cliquant sur le GameObject  _"GameCanvas > **Game Container**"_. Un nombre impair supérieur ou égale à 3 est recommandé pour le nombre de cases par ligne.
- Par défaut, un joueur ne peut avoir un high score que s'il gagne la partie.

## Bugs connus
- Si la grille est découpée en trop de carrés et que la texture est trop petite, le jeu plante.

## Améliorations possibles
- Adapter le jeu à toutes les résolutions.
- Ajouter des sons.
- Ajouter des puzzles et un système de difficulté.
- Ajouter un menu "Options".
- Ajouter un classement en ligne.
- Le code peut être simplifié et optimisé.

## Crédits
- [tada2.wav by Jobro](https://freesound.org/people/jobro/sounds/60444/#)
- [Retro You Lose SFX by SunTemple](https://freesound.org/people/suntemple/sounds/253174/#)
---
# Test de développement 

Le but de ce test est de déterminer votre niveau de maitrise d'Unity, du C#, du GamePlay et de la gestion de projet mobile.

## Créer un jeu de taquin.

Vous devez créer un jeu de taquins dans une nouvelle scène Unity et la connecter à la "Home". Les règles du jeu sont les règles traditionnelles  (https://fr.wikipedia.org/wiki/Taquin ) adaptées pour 9 cases et utilisant une image en guise de cible finale. Comme pour les jeux de taquins pour enfants. 

<img src="https://github.com/jfc-babaoo/Test/raw/release/README.Assets/taquin_kid.jpg" width="200">

⚠️ Attention : La case du centre sera la case vide à la résolution pour notre jeu. 



## Les instructions sont les suivantes
- "checkout" du dépôt git https://github.com/jfc-babaoo/Test.git
- créer une nouvelle scène et la rentre accessible par le bouton sur la "home"
- le jeu doit commencer par un tirage aléatoire des positions de cases
- le tirage doit être soluble
- le joueur doit déplacer les cases par glisser-déposer tactile
- le joueur à 3 minutes pour réussir
- le meilleur score doit être affiché sur la home et enregistré

## Particularité du gameplay
- le joueur bouge des taquins sur l'interface utilisateur et doit avoir une bonne expérience/impression tactile
- le mouvement des taquins se reproduit sur les éléments de la scène sous-jacente  soit en 2D, en 2.5D ou en 3D (au choix)

<img src="https://github.com/jfc-babaoo/Test/raw/release/README.Assets/render.png" width="200">

## Compilation IOS et Android
le jeu doit compiler sous IOS et sous Android. Bien-sûr, le logo utilisé pour les taquins doit correspondre à la plate-forme ciblée.
