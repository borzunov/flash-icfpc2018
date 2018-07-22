// Open (or create) the database
import Dexie from 'dexie'

const db = new Dexie("models_database");
db.version(1).stores({
  models: 'name, value'
});

export default db.models;