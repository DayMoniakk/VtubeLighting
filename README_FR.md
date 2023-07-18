
# VtubeLighting

Application pour Vtubers permettant d'implémenter l'éclairage de l'écran sur votre avatar sans avoir de bordure jaune autour de votre fenêtre !

<img src="screenshots/Screenshot_01.png" width="45%"></img> 
<img src="screenshots/Screenshot_02.png" width="45%"></img> 
<img src="screenshots/Screenshot_03.png" width="45%"></img> 
<img src="screenshots/Screenshot_04.png" width="45%"></img>

## Fonctionnalités

- Léger (environ 5% de processeur sur mon Ryzen 5 3600 et 160MB de ram)
- Pas de bordure jaunes dsitrayantes autour de la fenêtre
- L'interface n'est pas visible dans le logiciel de stream
- 100% Open Source et 100% Gratuit, POUR TOUJOURS
- Peut être facilement traduit (regardez dans `VtubeLighting_Data\Resources\Translations`)

## Prérequis

* Un logiciel de Vtubing compatible avec Spout2 (Vtube Studio, VseeFace, ...)
* Un logiciel de stream compatible avec Spout2 (OBS Studio avec un plugin externe)
* Une carte graphique compatible avec DirectX 11 ou 12
* VtubeLighting requiert l'utilisation d'une camera virtuelle donc vous ne pourrez pas l'utiliser pour autre chose, mais c'est pas un soucis car la plupart des applications de vtubing ont des caméras virtuelles intégrées

## Instructions

N'ayez pas peur si les instructions suivantes ont l'air compliqué, une fois tout paramétré correctement ça sera plus facile =)

Si vous préferez regarder un tutoriel vidéo au lieu de lire tout ça vous pouvez regarder [cette vidéo](https://www.youtube.com/watch?v=023tl29R6YA)

1. Téléchargez **VtubeLighting** depuis [Github Releases](https://github.com/DayMoniakk/VtubeLighting/releases)

2. Ouvrez **VtubeLighting** (vous pouvez le passer en Français depuis "Language")

3. In *Mise en place de l'éclairage* vous devez dire au logiciel comment récuperer votre avatar :
Dans Vtube Studio ouvrez le menu, cliquez sur **l'icone de caméra** and descendez jusqu'à ce que vous voyez *Spout2 Config*. Si vous ne l'avez déjà pas installé cliquez sur **Install Spout2**. Ensuite cliquez sur **Activate Spout2**.

4. Maintenant revenez dans **VtubeLighting**, cliquez sur le **bouton rafraichir**. Si Spout2 est correctement installé et activé vous verrez que **VTubeStudioSpout** est selectionné automatiquement pour vous.

5. Maintenant que nous avons l'avatar dans le logiciel il est temps de s'occuper de l'éclairage ! Lancez OBS Studio, créez une **nouvelle scène**, cette scène est responsable d'illuminer votre avatar. Tout ce que vous mettrez dedans influencera l'éclairage, par exemple vous pouvez mettre votre **Capture de jeu** dedans.

6. Maintenant regardez en bas à droite de OBS, il y a un bouton appelé **Demarrer la caméra virtuelle**: cliquez sur **l'icone de roue crantée** à côté. Dans **Type de sortie** sélectionnez **Scène** et dans **Sélection de sortie** sélectionnez la scène que vous venez de créer. Cliquez **OK** et **Démarrer la caméra virtuelle**.

7. Retournez dans **VtubeLighting**, dans **Mise en place de l'éclairage** il y a une option appelée **Source de la lumière** : utilisez le bouton rafraichir et vous aurez automatiquement **OBS Virtual Camera** de selectionné.

8. Vous pouvez enfin utiliser le bouton **Activer l'éclairage** ! Votre scène OBS est désormais visible dans l'application. La dernière chose à faire est de modifier la valeur de **Opacité** et votre avatar désormais reçois l'éclairage venant de OBS. Vous pouvez contrôler combien d'éclairage vous voulez en utilisant le slider **Intensité**.

9. Vous avez aussi une option appelée **Fréquence de mise à jour**, c'est simplement pour contrôler combiens de fois par secondes la lumière doit être recalculée. Plus haute est la valeur mieux sera l'effet mais pire seront les performances.

10. Une fois que vous êtes content du résultat il est temps de renvoyer l'avatar dans OBS.
Installez le plugin [Spout2 Plugin for OBS Studio](https://docs.offworld.live/#/obs-spout-plugin/README), ajoutez une **Spout2 Capture** dans votre scène de stream et selectionnez **VtubeLighting** dans **SpoutSenders**.
N'oubliez pas de changer **Composite Mode** en **Default**, si vous ne le faites pas vous aurez juste votre avatar avec un fond noir au lieu d'avoir un arrière plan transparent.

11. La dernière étape est d'ajouter votre scène d'éclairage à votre scène de stream, VtubeLighting renvoie uniquement l'avatar, pas la scène que vous lui envoyez. Cliquez sur **l'icone plus**, cliquez ensuite sur **Scène** et selectionnez votre scène d'éclairage. Cela permet d'afficher tout le contenu de votre scène d'éclairage dans la scène de stream.

## Notes

* L'application est crée avec `Unity 2022.3.4f1` en utilisant la built-in render pipeline et le shadergraph, n'hésitez pas à modifier VtubeLighting !
* Si vous prévoyez de modifier l'application faites attention à inclure les traductions du dossier `VtubeLighting\Assets\Translations` à `VtubeLighting_Data\Resources\Translations` après avoir exporté. Au passage, la librairie KlakSpout utilisée pour gérer Spout2 est directement sotckée dans les fichier du projet au lieu de l'endroit habituel car j'ai dû fixer un bug lié aux nouvelles versions de Unity.
## Credits

* VtubeLighting n'existerait pas sans [KlakSpout](https://github.com/keijiro/KlakSpout) crée par [Keijiro Takahashi](https://github.com/keijiro).
* La police d'écriture [Montserrat](https://github.com/JulietaUla/Montserrat) crée par [JulietaUla](https://github.com/JulietaUla)
* [Unity Engine](https://unity.com/) et son fameux splash screen obligatoire =p
* Logo par [Bing AI](https://www.bing.com/?/ai) (il m'a dit qu'il apprécierait d'être crédité =D)

## FAQ

#### Pourquoi avoir crée cette application ?

Avoir ce genre d'effet de lumière est quelque chose que j'ai toujours voulu, avant VtubeLighting il n'y avais seulement que 2 façons de le faire :
* Utiliser **Display Lighting** dans Vtube Studio mais vous aurez une bordure jaune autour de la fenêtre, ce qui est très distrayant !
* Utiliser **StreamFX** qui est dorénavant payant :/
Donc qu'est ce qu'un dévelopeur fait quand l'outil dont il a besoin n'existe pas ? Il le crée lui même et c'est exactemenet ce que j'ai fait !
Et je veux que tout le monde puisse également en profiter, nous les vtubers sommes une sortie de grande famille, on doit s'entraîder les uns les autres ♡

#### Pourquoi ai-je un avertissement au moment de lancer l'application ?

C'est parce que **Windows Smart Screen** vous averti que le logiciel provient d'une source inconnue. Pour éviter cela les dévelopeurs doivent signer leur applications, c'est un processus coûteux que je ne veux pas faire.
Il n'y a aucun virus ici, mais si vous ne me faites pas confiance c'est totalement compréhensible, vous n'avez qu'à compiler votre propre éxécutable ou alors n'utilisez tout simplement pas VtubeLighting.

#### Y'a t'il une version Linux ou Mac ?

Malheureusement non, je n'ai pas de Linux ou Mac et je refuse de vous donner quelque chose que je ne peux pas tester moi même.
D'ailleurs Spout2 requiert DirectX 11 ou 12, je n'ai aucune idée si Mac supporte ça.
Mais le projet est **complètement Open Source** donc n'hésitez pas à essayer par vous même.

#### Est-ce que je peux l'utiliser à des fins commerciales ?

Faites **ce que vous voulez** avec VtubeLighting, c'est **totalemenet gratuit** et ça le restera **pour toujours** !
Tout ce que je demande c'est d'être crédité si vous créez un nouveau logiciel à partir de mon code, ça sera gentil de votre part :)

#### Comment traduire l'application dans une autre langue ?

Vous pouvez dupliquer le fichier `french.json` dans `VtubeLighting_Data\Resources` et éditez les lignes. VtubeLighting scanner automatiquement ce dossier à la recherche de nouvelles traductions.

### D'autres questions ou problème ?

N'hésitez pas à créer un post dans [discussions](https://github.com/DayMoniakk/VtubeLighting/discussions) si vous avez un compte Github, sinon vous pouvez m'envoyer un message sur [Twitter](https://twitter.com/DayMoniakk)
