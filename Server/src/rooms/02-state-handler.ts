import { Room, Client } from "colyseus";
import { Schema, type, MapSchema } from "@colyseus/schema";

export class Player extends Schema {
    @type("uint8")
    loss = 0;

    @type("int8")
    maxHp = 0;

    @type("int8")
    currentHp = 0;

    @type("number")
    speed = 0;

    @type("number")
    pX = Math.floor(Math.random() * 50) -25;

    @type("number")
    pY = 0;

    @type("number")
    pZ = Math.floor(Math.random() * 50) -25;

    @type("number")
    vX = 0;

    @type("number")
    vY = 0;

    @type("number")
    vZ = 0;

    @type("number")
    rX = 0;

    @type("number")
    rY = 0;
}

export class State extends Schema {
    @type({ map: Player })
    players = new MapSchema<Player>();

    something = "This attribute won't be sent to the client-side";

    createPlayer(sessionId: string, data: any) {
        const player = new Player();
        player.maxHp = data.hp;
        player.currentHp = data.hp;
        player.speed = data.speed;

        this.players.set(sessionId, player);
    }

    removePlayer(sessionId: string) {
        this.players.delete(sessionId);
    }

    movePlayer (sessionId: string, data: any) {
        const player = this.players.get(sessionId);

       player.pX = data.pX;
       player.pY = data.pY;
       player.pZ = data.pZ;
       player.vX = data.vX;
       player.vY = data.vY;
       player.vZ = data.vZ;
       player.rX = data.rX;
       player.rY = data.rY;
    }
}

export class StateHandlerRoom extends Room<State> {
    maxClients = 2;

    pointsTeamA = [[1, 2], 
                   [10, 3]]; 


    pointsTeamB = [[30, 2], 
                   [30, 3]]; 



    onCreate (options) {
        console.log("StateHandlerRoom created!", options);

        this.setState(new State());

        this.onMessage("move", (client, data) => {
            // console.log("StateHandlerRoom received message from", client.sessionId, ":", data);
            this.state.movePlayer(client.sessionId, data);
        });

        this.onMessage("shoot", (client, data) => {
            this.broadcast("Shoot", data, {except: client});
        });

        this.onMessage("sitState", (client, data) => {
            this.broadcast("SitState", data, {except: client})
        });

        this.onMessage("damage", (client, data) => {
            const    clientID =  data.id;

            const player = this.state.players.get(clientID);

            let hp = player.currentHp - data.value;

            if(hp > 0){
                player.currentHp = hp;
                return;
            }
            player.currentHp -= data.value;

            player.loss++;
            player.currentHp = player.maxHp;

            for(var i = 0; i < this.clients.length; i++){
                if(this.clients[i].id != clientID) continue;

                const x = Math.floor(Math.random() * 50) -25;
                const z = Math.floor(Math.random() * 50) -25;

                // if(clientID < 1 ){
                //     const x =  this.pointsTeamA[0]
                // }

                // if(clientID == 1 ){
                //     const x =  this.pointsTeamA[1]
                // }


                const massage = JSON.stringify({x,z});
                client.send("Restart", x);
            }
        });
    }

    onAuth(client, options, req) {
        return true;
    }

    onJoin (client: Client, data: any) {
       if(this.clients.length > 1)this.lock;
       
        client.send("hello", "world");
        this.state.createPlayer(client.sessionId, data);
    }

    onLeave (client) {
        this.state.removePlayer(client.sessionId);
    }

    onDispose () {
        console.log("Dispose StateHandlerRoom");
    }

}
