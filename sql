CREATE TABLE players (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    progress_json JSONB
);

CREATE TABLE digibees (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    player_id UUID REFERENCES players(id),
    line TEXT,
    form INTEGER,
    level INTEGER,
    ivs_json JSONB,
    evs_json JSONB,
    nature_json JSONB,
    stats_json JSONB,
    pp_json JSONB
);

CREATE TABLE inventory (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    player_id UUID REFERENCES players(id),
    items_json JSONB
);

CREATE TABLE relics (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    player_id UUID REFERENCES players(id),
    relics_json JSONB
);

CREATE TABLE trades (
    id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    initiator_id UUID REFERENCES players(id),
    receiver_id UUID REFERENCES players(id),
    offer_json JSONB,
    status TEXT DEFAULT 'pending'
);

-- Enable RLS
ALTER TABLE players ENABLE ROW LEVEL SECURITY;
CREATE POLICY "Players own their data" ON players FOR ALL USING (auth.uid() = id);

-- Repeat for other tables: USING (auth.uid() = player_id)