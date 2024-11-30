import './App.css';
import Matches from "./pages/Matches";
import React from 'react';
import { Route, Routes, useLocation } from 'react-router-dom';
import SingleMatch from './pages/SingleMatch';
import SingleMatchStandings from './pages/SingleMatchStandings';
import Competitions from './pages/Competitions';
import TeamsInCompetition from './pages/TeamsInCompetition';
import Team from './pages/Team';
import TopScorers from './pages/TopScorers';
import Player from './pages/Player';
import TeamRoster from './pages/TeamRoster';
import 'bootstrap/dist/css/bootstrap.min.css';
import Events from './pages/Events';
import SingleEvent from './pages/SingleEvent';
import CreateUser from './pages/CreateUser';
import LoginUser from "./pages/LoginUser";
import AddEventAdmin from './pages/AddEventAdmin';
import Navbar from "./components/Navbar";
import Footer from "./components/Footer";

function App() {
  const location = useLocation();

  return (
    <div>
      {location.pathname !== '/user/login' && <Navbar />}

      <main className="main-content" style={{minHeight : "100vh"}}>
        <Routes>
          <Route path="/" element={<Matches />} />
          <Route path="/matches" element={<Matches />} />
          <Route path="/matches/:id" element={<SingleMatch />} />
          <Route path="/matches/:id/standings" element={<SingleMatchStandings />} />
          <Route path="/competitions" element={<Competitions />} />
          <Route path="/competitions/:id" element={<TeamsInCompetition />} />
          <Route path="/team/:id" element={<Team />} />
          <Route path="/team/:id/roster" element={<TeamRoster />} />
          <Route path="/player" element={<TopScorers />} />
          <Route path="/player/:id" element={<Player />} />
          <Route path="/events" element={<Events />} />
          <Route path="/events/:id" element={<SingleEvent />} />
          <Route path="/user/register" element={<CreateUser />} />
          <Route path="/user/login" element={<LoginUser />} />
          <Route path="/admin/event/add" element={<AddEventAdmin />} />
        </Routes>
      </main>

      {location.pathname !== '/user/login' && <Footer />}
    </div>
  );
}

export default App;
