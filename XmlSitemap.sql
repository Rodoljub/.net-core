
DECLARE @precision DECIMAL(5,5) = 0.6

DECLARE @Xml XML = null, @XMLStr NVARCHAR(MAX) = '<?xml version="1.0" encoding="UTF-8"?>';
WITH XMLNAMESPACES (DEFAULT 'http://www.sitemaps.org/schemas/sitemap/0.9', 'http://www.google.com/schemas/sitemap-image/1.1' AS image)

SELECT @Xml = 

	(SELECT  
		CONCAT('https://www.occpy.com/(a:aa/', Item.ID, ')') as loc,
			(SELECT CONCAT('https://media.occpy.com/images/', ID, '.jpeg')
			FROM [db_a8d0b3_creativecraftsdb].[dbo].[Files] 
			WHERE IsDeleted = 0 and Item.FileID = ID) as [image/imageloc],
		CONVERT(varchar, Item.LastModified, 31) as  lastmod,
		(SELECT CONCAT(jt.[description], CASE WHEN jt.[description] <> ''  THEN ' , ' ELSE '' END , jt.[tags])
				FROM
				(SELECT 
				 (SELECT TOP (1)
					JSON_VALUE([desc], '$.captions[0].text') as caption_text
					WHERE CAST(JSON_VALUE([desc], '$.captions[0].confidence') AS DECIMAL(5,5)) > @precision) as [description],

					(SELECT STRING_AGG([Name], ', ') FROM
						(SELECT [NAME], CAST(Confidence AS DECIMAL(5,5)) Confidence 
						FROM OPENJSON([tags], '$')
						WITH (
							[Name] NVARCHAR(50) '$.name',
							Confidence NVARCHAR(50) '$.confidence'
							)) t
					WHERE t.Confidence > @precision) as [tags]

				FROM OPENJSON((SELECT REPLACE(REPLACE((
					SELECT TOP (1) [ImageAnalysis]
					FROM [db_a8d0b3_creativecraftsdb].[dbo].[FileDetails]
					WHERE ID = [File].FileDetailsID AND IsDeleted = 0
				), '"', ''), '\u0022', '"')))
				WITH
				(
					[desc] NVARCHAR(MAX) '$.description' AS JSON,
					[tags] NVARCHAR(MAX) '$.tags' AS JSON

				)) as jt) as [image/imagecaption]
	FROM 
	[db_a8d0b3_creativecraftsdb].[dbo].[Items] as Item,
	[db_a8d0b3_creativecraftsdb].[dbo].[Files] as [File]
	WHERE 
		Item.IsDeleted = 0 AND [File].IsDeleted = 0 
		AND Item.FileID = [File].ID 
	FOR XML PATH ('url'), ROOT('urlset'))


DECLARE @XmlString VARCHAR(MAX);

SET @XmlString = CONCAT(@XMLStr , REPLACE(REPLACE(CAST(@XML AS NVARCHAR(MAX)), 'imageloc', 'image:loc'), 'imagecaption','image:caption'));

SELECT CAST( @XmlString AS XML)




