INSERT INTO userlevels_highscores
(
  userid,
  levelid,
  d0_time,
  d0_lastplayed,
  d1_time,
  d1_lastplayed,
  d2_time,
  d2_lastplayed,
  d3_time,
  d3_lastplayed
)
VALUES
(
  :uid,
  :lid,
  :d0t2,
  :d0p2,
  :d1t2,
  :d1p2,
  :d2t2,
  :d2p2,
  :d3t2,
  :d3p2
)
ON DUPLICATE KEY UPDATE

  d0_time = LEAST(:d0t1,d0_time),
  d0_lastplayed = GREATEST(:d0p1,d0_lastplayed),
  d1_time = LEAST(:d1t1,d1_time),
  d1_lastplayed = GREATEST(:d1p1,d1_lastplayed),
  d2_time = LEAST(:d2t1,d2_time),
  d2_lastplayed = GREATEST(:d2p1,d2_lastplayed),
  d3_time = LEAST(:d3t1,d3_time),
  d3_lastplayed = GREATEST(:d3p1,d3_lastplayed)