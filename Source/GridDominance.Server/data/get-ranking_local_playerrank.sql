SELECT
	usr.userid,
	usr.username,
	usr.:__FIELD_SCORE__ as totalscore,
	usr.:__FIELD_TIME__ as totaltime,
	(
		SELECT COUNT(*)  + 1
		FROM users othr
		WHERE
		      (othr.:__FIELD_SCORE__ > usr.:__FIELD_SCORE__) OR
		      (othr.:__FIELD_SCORE__ = usr.:__FIELD_SCORE__ AND othr.:__FIELD_TIME__ < usr.:__FIELD_TIME__ ) OR
		      (othr.:__FIELD_SCORE__ = usr.:__FIELD_SCORE__ AND othr.:__FIELD_TIME__ = usr.:__FIELD_TIME__ AND othr.userid < usr.userid )
	) AS "rank"

FROM users usr

WHERE usr.userid = :uid
