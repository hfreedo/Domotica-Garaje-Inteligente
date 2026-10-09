document.getElementById('login').addEventListener('submit',async e=>{
  e.preventDefault();const error=document.getElementById('error');error.textContent='Comprobando…';
  try{const r=await fetch('/api/login',{method:'POST',headers:{'Content-Type':'application/json','X-Garage-Request':'1'},body:JSON.stringify({pin:document.getElementById('pin').value})});
    const d=await r.json();if(!r.ok)throw Error(d.error);location.replace('/');
  }catch(e){error.textContent=e.message;}
});
