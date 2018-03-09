SELECT
  ul.id, ul.name, ul.userid, ux.username,
  ul.upload_timestamp, ul.datahash, ul.upload_version,
  ul.grid_width, ul.grid_height, ul.stars,
  ul.d0_completed, ul.d0_played, ul.d0_bestuserid, u0.username AS d0_bestusername, ul.d0_besttime, ul.d0_besttimestamp,
  ul.d1_completed, ul.d1_played, ul.d1_bestuserid, u1.username AS d1_bestusername, ul.d1_besttime, ul.d1_besttimestamp,
  ul.d2_completed, ul.d2_played, ul.d2_bestuserid, u2.username AS d2_bestusername, ul.d2_besttime, ul.d2_besttimestamp,
  ul.d3_completed, ul.d3_played, ul.d3_bestuserid, u3.username AS d3_bestusername, ul.d3_besttime, ul.d3_besttimestamp

FROM userlevels AS ul

  LEFT JOIN users AS ux ON ux.userid=ul.userid

  LEFT JOIN users AS u0 ON u0.userid=ul.d0_bestuserid
  LEFT JOIN users AS u1 ON u1.userid=ul.d1_bestuserid
  LEFT JOIN users AS u2 ON u2.userid=ul.d2_bestuserid
  LEFT JOIN users AS u3 ON u3.userid=ul.d3_bestuserid

WHERE
  ul.upload_timestamp IS NOT NULL AND
  ul.upload_decversion <= :dvs AND
  (
    LOWER(ux.username)=LOWER(:search1) OR
    ul.id = :search2 OR
    LOWER(ul.name) LIKE (:search3)
  )

ORDER BY
  stars DESC,
  upload_timestamp DESC


LIMIT  :lim
OFFSET :off