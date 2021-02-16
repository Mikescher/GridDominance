SELECT
	usr.userid,
	usr.username,
	usr.score_stars as totalscore,
	usr.time_total as totaltime,
	(
		SELECT COUNT(*) + 1
		FROM users othr
		WHERE (othr.score_stars > usr.score_stars) OR (othr.score_stars = usr.score_stars AND othr.userid < usr.userid )
	) AS "rank"

FROM users usr

WHERE usr.userid = :uid
