
DROP DATABASE IF EXISTS grid_dominance;
CREATE DATABASE IF NOT EXISTS grid_dominance;

USE grid_dominance;




DROP TABLE IF EXISTS users;
CREATE TABLE IF NOT EXISTS users
(
  userid                  int(11)       NOT NULL AUTO_INCREMENT,
  username                varchar(64)   NOT NULL,
  password_hash           char(128)     NOT NULL,
  is_auto_generated       boolean       NOT NULL,

  score                   int(11)       NOT NULL DEFAULT 0,
  score_w1                int(11)       NOT NULL DEFAULT 0,
  score_w2                int(11)       NOT NULL DEFAULT 0,
  score_w3                int(11)       NOT NULL DEFAULT 0,
  score_w4                int(11)       NOT NULL DEFAULT 0,
  score_sccm              int(11)       NOT NULL DEFAULT 0,
  score_stars             int(11)       NOT NULL DEFAULT 0,
  mpscore                 int(11)       NOT NULL DEFAULT 0,
  time_total              int(11)       NOT NULL DEFAULT 0,
  time_w1                 int(11)       NOT NULL DEFAULT 0,
  time_w2                 int(11)       NOT NULL DEFAULT 0,
  time_w3                 int(11)       NOT NULL DEFAULT 0,
  time_w4                 int(11)       NOT NULL DEFAULT 0,

  revision_id             int(11)       UNSIGNED NOT NULL DEFAULT 0,

  creation_time           timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP,

  last_online             timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  app_version             varchar(24)   NOT NULL,
  device_name             varchar(128)  NOT NULL,
  device_version          varchar(128)  NOT NULL,
  unlocked_worlds         varchar(1024) NOT NULL,
  device_resolution       varchar(24)   NOT NULL,
  app_type                varchar(128)  NOT NULL DEFAULT '?',

  ping_counter            int           UNSIGNED NOT NULL DEFAULT 0,

  PRIMARY KEY (userid)
);




DROP TABLE IF EXISTS level_highscores;
CREATE TABLE IF NOT EXISTS level_highscores 
(
  userid        int(11)      NOT NULL,
  shortid       smallint(6)  NOT NULL,
  difficulty    tinyint(4)   NOT NULL,
  best_time     int(11)      NOT NULL,
  last_changed  timestamp    NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,

  PRIMARY KEY (userid, shortid, difficulty),

  KEY specific_level (shortid,   difficulty),
  KEY order_index    (best_time, last_changed)
);




DROP TABLE IF EXISTS cache_levels;
CREATE TABLE IF NOT EXISTS cache_levels
(
  levelid           char(38)   NOT NULL,
  difficulty        tinyint(4) NOT NULL,
  best_time         int(11)    NOT NULL,
  best_userid       int(11)    NOT NULL,
  best_last_changed timestamp  NOT NULL,
  completion_count  int(11)    NOT NULL,

  PRIMARY KEY (levelid,difficulty)
);




DROP TABLE IF EXISTS error_log;
CREATE TABLE IF NOT EXISTS error_log
(
  error_id              int(11)       NOT NULL AUTO_INCREMENT,
  userid                int(11)       NOT NULL,
  password_verified     boolean       NOT NULL,
  screen_resolution     varchar(256)  NOT NULL,
  app_version           varchar(24)   NOT NULL,
  exception_id          varchar(256)  NOT NULL,
  exception_message     mediumtext    NOT NULL,
  exception_stacktrace  text          NOT NULL,
  timestamp             timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  additional_info       text          NOT NULL,
  acknowledged          boolean       NOT NULL DEFAULT FALSE,

  PRIMARY KEY (error_id),

  KEY acknowledged (acknowledged),
  KEY userid       (userid),
  KEY app_version  (app_version),
  KEY exception_id (exception_id)
);




DROP TABLE IF EXISTS idmap;
CREATE TABLE idmap
(
  levelid  char(38)      NOT NULL,
  worldid  char(38)      NOT NULL,
  id       varchar(7)    NOT NULL,
  name     varchar(128)  NOT NULL,
  shortid  smallint(6)   NOT NULL,

  PRIMARY KEY (levelid)
);




DROP TABLE IF EXISTS runlog_volatile;
CREATE TABLE runlog_volatile (
  id              int(11)       NOT NULL AUTO_INCREMENT,
  action          char(32)      NOT NULL,
  exectime        timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  duration        int(11)       NOT NULL,

  PRIMARY KEY (id)
);




DROP TABLE IF EXISTS runlog_history;
CREATE TABLE runlog_history
(
  id              int(11)       NOT NULL AUTO_INCREMENT,
  exectime        timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  action          char(32)      NOT NULL,
  min_timestamp   timestamp     NULL DEFAULT NULL,
  max_timestamp   timestamp     NULL DEFAULT NULL,
  count           int(11)       NOT NULL,
  duration        bigint(20)    NOT NULL,
  duration_min    int(11)       NOT NULL,
  duration_max    int(11)       NOT NULL,
  duration_avg    double        NOT NULL,
  duration_median double        NOT NULL,

  PRIMARY KEY (id)
);


DROP TABLE IF EXISTS session_history;
CREATE TABLE session_history
(
  id                  int(11)       NOT NULL AUTO_INCREMENT,
  time                timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP,
  sessioncount_active int(11)       NOT NULL,
  sessioncount_total  int(11)       NOT NULL,

  PRIMARY KEY (id)
);

DROP TABLE IF EXISTS stats_history;
CREATE TABLE stats_history
(
  id                    int(11)       NOT NULL AUTO_INCREMENT,
  exectime              timestamp     NOT NULL DEFAULT CURRENT_TIMESTAMP,

  active_users_per_day  int(11)       NOT NULL,

  user_amazon           int(11)       NOT NULL,
  user_android_full     int(11)       NOT NULL,
  user_android_iab      int(11)       NOT NULL,
  user_ios              int(11)       NOT NULL,
  user_winphone         int(11)       NOT NULL,

  unlocks_w1            int(11)       NOT NULL,
  unlocks_w2            int(11)       NOT NULL,
  unlocks_w3            int(11)       NOT NULL,
  unlocks_w4            int(11)       NOT NULL,
  unlocks_mp            int(11)       NOT NULL,
  unlocks_sccm          int(11)       NOT NULL,

  user_topscore         int(11)       NOT NULL,

  user_current_version  int(11)       NOT NULL,
  user_old_version      int(11)       NOT NULL,

  PRIMARY KEY (id)
);

DROP TABLE IF EXISTS userlevels;
CREATE TABLE userlevels
(
  id                 bigint(20)          UNSIGNED NOT NULL AUTO_INCREMENT,

  name               varchar(64)         NULL DEFAULT NULL,
  userid             int(10)             UNSIGNED NOT NULL,
  creation_timestamp timestamp           NOT NULL DEFAULT CURRENT_TIMESTAMP,
  upload_timestamp   timestamp           NULL DEFAULT NULL,
  upload_version     varchar(24)         NULL DEFAULT NULL,
  upload_decversion  bigint(20)          UNSIGNED NULL DEFAULT NULL,
  grid_width         smallint(6)         NULL DEFAULT NULL,
  grid_height        smallint(6)         NULL DEFAULT NULL,
  author_time        int(10)             NULL DEFAULT NULL,
  datahash           varchar(64)         NULL DEFAULT NULL,
  filesize           int(10)             NULL DEFAULT NULL,

  stars              int(10)             NOT NULL DEFAULT '0',

  d0_completed       int(10)             NOT NULL DEFAULT '0',
  d0_played          int(10)             NOT NULL DEFAULT '0',
  d0_bestuserid      int(10)             NULL DEFAULT NULL,
  d0_besttime        int(10)             NULL DEFAULT NULL,
  d0_besttimestamp   timestamp           NULL DEFAULT NULL,

  d1_completed       int(10)             NOT NULL DEFAULT '0',
  d1_played          int(10)             NOT NULL DEFAULT '0',
  d1_bestuserid      int(10)             NULL DEFAULT NULL,
  d1_besttime        int(10)             NULL DEFAULT NULL,
  d1_besttimestamp   timestamp           NULL DEFAULT NULL,

  d2_completed       int(10)             NOT NULL DEFAULT '0',
  d2_played          int(10)             NOT NULL DEFAULT '0',
  d2_bestuserid      int(10)             NULL DEFAULT NULL,
  d2_besttime        int(10)             NULL DEFAULT NULL,
  d2_besttimestamp   timestamp           NULL DEFAULT NULL,

  d3_completed       int(10)             NOT NULL DEFAULT '0',
  d3_played          int(10)             NOT NULL DEFAULT '0',
  d3_bestuserid      int(10)             NULL DEFAULT NULL,
  d3_besttime        int(10)             NULL DEFAULT NULL,
  d3_besttimestamp   timestamp           NULL DEFAULT NULL,

  PRIMARY KEY (id),

  INDEX stars (stars),
  INDEX upload_timestamp (upload_timestamp),
  INDEX userid (userid)
);

DROP TABLE IF EXISTS userlevels_highscores;
CREATE TABLE userlevels_highscores
(
  userid            int(11)           NOT NULL,
  levelid           bigint(20)        NOT NULL,

  d0_time           int(11)           NULL DEFAULT NULL,
  d0_lastplayed     timestamp         NULL DEFAULT NULL,

  d1_time           int(11)           NULL DEFAULT NULL,
  d1_lastplayed     timestamp         NULL DEFAULT NULL,

  d2_time           int(11)           NULL DEFAULT NULL,
  d2_lastplayed     timestamp         NULL DEFAULT NULL,

  d3_time           int(11)           NULL DEFAULT NULL,
  d3_lastplayed     timestamp         NULL DEFAULT NULL,

  starred           bit(1)            NOT NULL DEFAULT b'0',

  PRIMARY KEY (userid, levelid)
);
