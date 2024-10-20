
import './App.css';
import Matches from "./pages/Matches"
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import SingleMatch from './pages/SingleMatch';
import SingleMatchStandings from './pages/SingleMatchStandings'
import Competitions from './pages/Competitions';
import TeamsInCompetition from './pages/TeamsInCompetition';
import Team from './pages/Team';
import TopScorers from './pages/TopScorers';
import Player from './pages/Player';
import TeamRoster from './pages/TeamRoster';
function App() {
  return (
    <Router>
      <div>
        <Routes>
          <Route path="/matches" element={<Matches />} />
          <Route path="/matches/:id" element={<SingleMatch />} />
          <Route path="/matches/:id/standings" element={<SingleMatchStandings />} />
          <Route path="/competitions" element={<Competitions />} />
          <Route path="/competitions/:id" element={<TeamsInCompetition />} />
          <Route path="/competitions/:id/topScorers" element={<TopScorers />} />
          <Route path="/team/:id" element={<Team />} />
          <Route path="/team/:id/roster" element={<TeamRoster />} />
          <Route path="/player/:id" element={<Player />} />

        </Routes>

        {/* Add more routes as needed */}

      </div>
    </Router>
  );
}

export default App;
