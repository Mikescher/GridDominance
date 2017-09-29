REPLACE INTO cache_levels 
(
	levelid,
	difficulty,
	best_time,
	best_userid,
	best_last_changed,
	completion_count
)

(
	SELECT
		idmap.levelid,
		diff_tab.difficulty,
		winner_tab.best_time,
		winner_tab.userid,
		winner_tab.last_changed,
		count_tab.counter
	FROM idmap

	INNER JOIN
	(
		SELECT 0 AS difficulty UNION 
		SELECT 1 AS difficulty UNION 
		SELECT 2 AS difficulty UNION 
		SELECT 3 AS difficulty
	) AS diff_tab

	INNER JOIN
	(
		SELECT 
			level_highscores.levelid, 
			level_highscores.difficulty, 
			COUNT(*) AS counter
		FROM level_highscores
		GROUP BY levelid, difficulty
	) AS count_tab
	ON 
		count_tab.levelid = idmap.levelid AND
		count_tab.difficulty = diff_tab.difficulty

	INNER JOIN
	(
		SELECT lh3.levelid, lh3.difficulty, lh3.best_time, lh3.userid, lh3.last_changed  FROM
		(
			SELECT lh2.levelid, lh2.difficulty, min(best_time) AS min_best_time
			FROM level_highscores AS lh2
			GROUP BY lh2.levelid, lh2.difficulty
		) AS lh4

		INNER JOIN level_highscores AS lh3
		ON 
			lh3.levelid = lh4.levelid AND 
			lh3.difficulty = lh4.difficulty AND 
			lh3.best_time = lh4.min_best_time
		
	) AS winner_tab
	ON 
		winner_tab.levelid = idmap.levelid AND
		winner_tab.difficulty = diff_tab.difficulty

	ORDER BY last_changed DESC
)