/** 
 * AssetFlow EMS - Online Demo Logic 
 */

document.addEventListener('DOMContentLoaded', () => {
    // 1. 初始化資料 (若 localStorage 為空)
    const MOCK_EQUIPMENTS = [
        { id: 101, name: "Canon EOS R10", category: "Camera", status: "Available", loc: "A1-Storage" },
        { id: 102, name: "Dell Latitude 7440", category: "Laptop", status: "In Use", loc: "Floor 2" },
        { id: 103, name: "Fluke 87V Multimeter", category: "Tool", status: "Maintenance", loc: "Lab B" }
    ];

    let equipments = JSON.parse(localStorage.getItem('gh_demo_eq')) || MOCK_EQUIPMENTS;
    let records = JSON.parse(localStorage.getItem('gh_demo_rec')) || [
        { id: 1, eqName: "Dell Latitude 7440", user: "User01", time: "2026-03-31 08:30", status: "Borrowed" }
    ];

    function save() {
        localStorage.setItem('gh_demo_eq', JSON.stringify(equipments));
        localStorage.setItem('gh_demo_rec', JSON.stringify(records));
        updateStats();
    }

    // 2. 渲染 UI
    function render() {
        const list = document.getElementById('eq-list');
        list.innerHTML = "";
        equipments.forEach(eq => {
            const tr = document.createElement('tr');
            const badge = eq.status === 'Available' ? 'bg-available' : (eq.status === 'In Use' ? 'bg-inuse' : 'bg-maint');
            tr.innerHTML = `
                <td class="fw-bold text-white">${eq.name}</td>
                <td><span class="text-muted small">${eq.category}</span></td>
                <td><span class="badge ${badge}">${eq.status}</span></td>
                <td>${eq.loc}</td>
                <td class="text-end">
                    ${eq.status === 'Available' ? `<button class="btn btn-sm btn-outline-primary" onclick="borrowItem(${eq.id})">借用</button>` : ''}
                    ${eq.status === 'In Use' ? `<button class="btn btn-sm btn-outline-info" onclick="returnItem(${eq.id})">歸還</button>` : ''}
                    <button class="btn btn-sm btn-outline-danger ms-1" onclick="deleteItem(${eq.id})"><i class="bi bi-trash"></i></button>
                </td>
            `;
            list.appendChild(tr);
        });

        const recList = document.getElementById('record-list');
        recList.innerHTML = "";
        records.forEach(r => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${r.eqName}</td>
                <td class="small text-muted">${r.user}</td>
                <td class="small text-muted">${r.time}</td>
                <td><span class="badge bg-dark">${r.status}</span></td>
            `;
            recList.appendChild(tr);
        });
    }

    function updateStats() {
        document.getElementById('stat-total').innerText = equipments.length;
        const inUse = equipments.filter(e => e.status === 'In Use').length;
        document.getElementById('stat-inuse').innerText = inUse;
        document.getElementById('stat-maint').innerText = equipments.filter(e => e.status === 'Maintenance').length;
        document.getElementById('stat-percent').innerText = equipments.length ? Math.round((inUse / equipments.length) * 100) : 0;
    }

    // 3. 全局操作 (掛載到 window)
    window.borrowItem = (id) => {
        const eq = equipments.find(e => e.id === id);
        eq.status = 'In Use';
        records.unshift({ id: Date.now(), eqName: eq.name, user: "Stranger", time: new Date().toLocaleString(), status: "Borrowed" });
        save();
        render();
    };

    window.returnItem = (id) => {
        const eq = equipments.find(e => e.id === id);
        eq.status = 'Available';
        records.unshift({ id: Date.now(), eqName: eq.name, user: "Stranger", time: new Date().toLocaleString(), status: "Returned" });
        save();
        render();
    };

    window.deleteItem = (id) => {
        if(confirm("確定刪除此設備？")) {
            equipments = equipments.filter(e => e.id !== id);
            save();
            render();
        }
    }

    window.showView = (view) => {
        document.querySelectorAll('.view-section').forEach(s => s.classList.add('d-none'));
        document.getElementById(`view-${view}`).classList.remove('d-none');
        document.querySelectorAll('.nav-item').forEach(n => n.classList.remove('active'));
    }

    // 啟動
    updateStats();
    render();
});
