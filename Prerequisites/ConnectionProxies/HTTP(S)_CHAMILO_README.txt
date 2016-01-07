---------------------
- Chamilo CP ReadMe -
---------------------


Introductie
-----------

Bij Chamilo draait alles in feite rond de repository. Hier kunnen verschillende types objecten op geplaatst worden, zoals kalender events, documenten en berichten.

Dit gebeurt telkens in 2 stappen:
 * Een POST van de data-form, waarna je een redirect krijgt met een repo_object ID.
 * Via deze redirect kan het object gepublished worden.

Voor de connection proxy moet dus het volgende gebeuren:
 * Bepalen wanneer repo_object extract kan worden.
 * Repo_object extracten.
 * Repo_object vervangen in GetData.
 * Repo_object vervangen in PostData.


Bepalen wanneer repo_object extract kan worden
----------------------------------------------

Dit kan achterhaald worden via Fiddler, door te kijken naar de redirects (HTTP 302). De location header van de response zal hoogst waarschijnlijk een repo_object bevatten. Gebruik dan de GetData van de request dat deze redirect veroorzaakt als search string in #region Fields.

Bv. string _calendar_search = "application=personal_calendar&go=publisher&repoviewer_action=creator&content_object_type=calendar_event";


Repo_object extracten
---------------------

In de SendAndReceive() methode is een stuk code aangebracht dat het repo_object correct zou moeten extracten. Eventueel is hier een kleine aanpassing nodig om bv. extra search strings toe te laten.


Repo_object verplaatsen in GetData
----------------------------------

@repo_object

De requests bij het posten van repository objecten moeten goed gecontrolleerd worden op 'repo_object=' queries in de GetData. Hier moet een vervanging gebeuren door de gelogde ID te vervangen door @repo_object.


Repo_object vervangen in PostData
---------------------------------

@repo_object_length, @repo_object

Voor het posten van een kalender evenement, moet het repo_object en z'n lengte toegevoegd worden in een speciale tekenreeks. De plaats waar dit gebeurt ziet er als volgt uit:

RelURL: index.php
Get: publisher&repo_object%5B0%5D=@repo_object&repoviewer_action=publisher
Post: share_users_and_groups_option=0&share_users_and_groups_elements_active_hidden=a%3A0%3A%7B%7D&share_users_and_groups_elements_search=&submit=Publiceer&_qf__publish=&ids=a%3A1%3A%7Bi%3A0%3Bs%3A@repo_object_length%3A%22@repo_object%22%3B%7D


Extra's
-------

Als er een document gepost wordt, dan dienen er custom headers toegevoegd te worden. Dit gebeurt automatisch bij ApplyPostData() en is afhankelijk van de _document_search customizatie-string.


