# UI-Testvertrag

Die Oberfläche stellt stabile `AutomationId`-Werte bereit. UI-Tests sollen diese
IDs statt sichtbarer Texte verwenden, damit Umformulierungen und Lokalisierung
keine Tests brechen.

## Navigation

- `Hauptnavigation`
- `NavigationTraining`
- `NavigationVokabeln`
- `NavigationVerlauf`
- `NavigationEinstellungen`

## Training

- `TrainingsUmfang`, `PriorisierteVokabeln`, `NurNeueVokabeln`
- `TrainingStarten`, `TrainingBeenden`, `Trainingsstatus`
- `TrainingsVokabelListe`

## Vokabeln

- `DeutschEingabe`, `EnglischEingabe`, `VokabelSuche`
- `NeueVokabel`, `VokabelSpeichern`, `VokabelLoeschen`
- `VokabelListe`, `VokabelAnzahl`, `PriorisierteVokabelAnzahl`
- Tagesgruppen: `VokabelTag-yyyyMMdd`, zugehörige Tabelle: `VokabelTag-yyyyMMdd-Liste`
- `NeueVokabelDeutsch`, `NeueVokabelEnglisch`

## Einstellungen und Verlauf

- `VokabelnExportieren`, `VokabelnImportieren`, `ImportFortschritt`
- `PriorisierungenZuruecksetzen`
- `FarbschemaHell`, `FarbschemaDunkel`, `FarbschemaSystem`
- `SitzungenListe`
- `VerlaufAnzahlSitzungen`, `VerlaufGeuebteVokabeln`, `VerlaufTrefferquote`

Die Seiten besitzen zusätzlich Konstruktoren, die ihr ViewModel entgegennehmen.
Dadurch können Seitentests ein ViewModel mit einem simulierten `IDataService`
verwenden, ohne die produktive Datenbank zu öffnen.

## Automatisierte Tests

Der exemplarische Test `ThemeSelectorServiceTests` prüft, dass die Anwendung
zwischen hellem und dunklem Farbschema wechselt und beide Auswahlen speichert.
Er verwendet ausschließlich Test-Doubles und verändert deshalb weder das echte
Fenster noch die lokalen Einstellungen des Benutzers.

```powershell
dotnet test Vokabeltrainer.Tests\Vokabeltrainer.Tests.csproj -c Debug
```

Der sichtbare End-to-End-Test startet die echte Anwendung und steuert die
Einstellungsseite über ihre `AutomationId`-Werte. Vor dem Start darf keine zweite
Instanz des Vokabeltrainers geöffnet sein.

```powershell
dotnet test Vokabeltrainer.Tests\Vokabeltrainer.Tests.csproj -c Debug --filter TestCategory=EndToEnd
```
