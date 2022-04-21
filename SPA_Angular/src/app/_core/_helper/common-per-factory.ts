const port = 107; // LOCALHOST
const portlocal = 5000; // LOCALHOST
const port_test = 1092; // SHC, CB, TSH, SPC
const ipserver = '10.4.5.174'
const iplocal = '10.4.5.132'
const ip = `${iplocal}:${portlocal}`;
const apiUrl = `http://${ip}`;
const apiUrlServer = `http://10.4.5.174:1066`;
const systemCode = 4;

export const commonPerFactory = {
  INK_SYSTEM_CODE: systemCode,
  apiUrlEC: `${apiUrl}/api/`,
  apiUrl: `${apiUrlServer}/api/`,
  hub: `${apiUrl}/ec-hub`
}