import * as React from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import styles from "../css/Standings.module.css"

function Standings({ data }) {

    console.log(data);

    return (
        <TableContainer component={Paper} style={{marginTop: 30}}>
            <Table sx={{ minWidth: 750 }} aria-label="simple table">
                <TableHead>
                    <TableRow>
                        <TableCell>Club</TableCell>
                        <TableCell align="center">MP</TableCell>
                        <TableCell align="center">W</TableCell>
                        <TableCell align="center">D</TableCell>
                        <TableCell align="center">L</TableCell>
                        <TableCell align="center">GF</TableCell>
                        <TableCell align="center">GA</TableCell>
                        <TableCell align="center">P</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {data.teamStandings.map((row, index) => (
                        <TableRow key={row.teamName} sx={{ '&:last-child td, &:last-child th': { border: 0 } }}>

                            <TableCell component="th" scope="row">
                                <div className={styles.position}>{index + 1}</div>
                                <img className={styles.teamCrest}  src={row.teamCrest} alt="Team Crest" />
                                {row.teamName}
                            </TableCell>
                            <TableCell align="center">{row.playedGames}</TableCell>
                            <TableCell align="center">{row.wins}</TableCell>
                            <TableCell align="center">{row.draws}</TableCell>
                            <TableCell align="center">{row.losses}</TableCell>
                            <TableCell align="center">{row.goalsFor}</TableCell>
                            <TableCell align="center">{row.goalsAgainst}</TableCell>
                            <TableCell align="center">{row.points}</TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}

export default Standings;